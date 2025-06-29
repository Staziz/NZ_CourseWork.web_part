using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WasteController : ControllerBase
    {
        private readonly WasteDbContext _db;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public WasteController(WasteDbContext db, IHttpClientFactory httpClientFactory, IWebHostEnvironment env, IConfiguration config)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
            _env = env;
            _config = config;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadWaste([FromForm] WasteUploadRequest req)
        {
            if (req.File == null || req.File.Length == 0)
                return BadRequest(new { message = "No file uploaded" });

            var uploads = Path.Combine(_env.ContentRootPath, "uploads");
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
            var fileName = Guid.NewGuid() + Path.GetExtension(req.File.FileName);
            var filePath = Path.Combine(uploads, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await req.File.CopyToAsync(stream);
            }

            // Forward to Python API
            var pythonApiUrl = _config["PythonApi:Url"] ?? "http://127.0.0.1:5000/predict";
            var client = _httpClientFactory.CreateClient();
            using var form = new MultipartFormDataContent();
            using var fs = System.IO.File.OpenRead(filePath);
            form.Add(new StreamContent(fs), "file", fileName);
            var response = await client.PostAsync(pythonApiUrl, form);
            string? result = null;
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsStringAsync();
            }

            var waste = new WasteItem
            {
                ImagePath = fileName,
                Status = result != null ? "classified" : "unclassified",
                Result = result,
                CreatedAt = DateTime.UtcNow
            };
            _db.WasteItems.Add(waste);
            await _db.SaveChangesAsync();
            return Ok(new { waste.Id, waste.ImagePath, waste.Status, waste.Result });
        }

        [HttpGet("queue")]
        public async Task<IActionResult> GetUnclassifiedQueue()
        {
            var queue = await _db.WasteItems.Where(w => w.Status == "unclassified").ToListAsync();
            return Ok(queue);
        }

        [HttpPost("manual-classify/{id}")]
        public async Task<IActionResult> ManualClassify(int id, [FromBody] ManualClassifyRequest req)
        {
            var waste = await _db.WasteItems.FindAsync(id);
            if (waste == null) return NotFound();
            waste.Status = "classified";
            waste.Result = req.Result;
            waste.Category = req.Category;
            await _db.SaveChangesAsync();
            return Ok(new { waste.Id, waste.Status, waste.Result, waste.Category });
        }
    }

    public class WasteUploadRequest
    {
        public IFormFile File { get; set; } = null!;
    }

    public class ManualClassifyRequest
    {
        public string Result { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}
