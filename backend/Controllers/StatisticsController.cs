using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class StatisticsController : ControllerBase
    {
        private readonly WasteDbContext _db;
        public StatisticsController(WasteDbContext db) { _db = db; }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string period = "day")
        {
            // Mock/demo data for demonstration
            var total = await _db.WasteItems.CountAsync();
            var correct = await _db.WasteItems.CountAsync(w => w.Status == "classified");
            var accuracy = total > 0 ? (double)correct / total : 1.0;
            var volumeByCategory = await _db.WasteItems
                .Where(w => w.Category != null)
                .GroupBy(w => w.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Category!, x => x.Count);
            var summary = new StatisticSummary
            {
                Period = period,
                Accuracy = accuracy,
                VolumeByCategory = volumeByCategory
            };
            return Ok(summary);
        }
    }
}
