using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AlertsController : ControllerBase
    {
        private readonly WasteDbContext _db;
        public AlertsController(WasteDbContext db) { _db = db; }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var alerts = await _db.Alerts.OrderByDescending(a => a.DetectedAt).ToListAsync();
            return Ok(alerts);
        }

        [HttpPost("assign/{id}")]
        public async Task<IActionResult> Assign(int id, [FromBody] AssignAlertRequest req)
        {
            var alert = await _db.Alerts.FindAsync(id);
            if (alert == null) return NotFound();
            alert.AssignedTo = req.AssignedTo;
            await _db.SaveChangesAsync();
            return Ok(alert);
        }

        [HttpPost("close/{id}")]
        public async Task<IActionResult> Close(int id)
        {
            var alert = await _db.Alerts.FindAsync(id);
            if (alert == null) return NotFound();
            alert.Closed = true;
            await _db.SaveChangesAsync();
            return Ok(alert);
        }
    }

    public class AssignAlertRequest
    {
        public string AssignedTo { get; set; } = string.Empty;
    }
}
