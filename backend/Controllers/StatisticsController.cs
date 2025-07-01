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
            // Waste volume by category
            var volumeByCategory = await _db.WasteItems
                .Where(w => w.Category != null)
                .GroupBy(w => w.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Category!, x => x.Count);

            // Sorting accuracy
            var total = await _db.WasteItems.CountAsync();
            var correct = await _db.WasteItems.CountAsync(w => w.IsCorrect == true);
            var accuracy = total > 0 ? (double)correct / total : 1.0;

            // Contamination alerts
            var alertsResolved = await _db.Alerts.CountAsync(a => a.Closed);
            var alertsUnresolved = await _db.Alerts.CountAsync(a => !a.Closed);

            // Manual vs. Automatic Classification
            var manualClassified = await _db.WasteItems.CountAsync(w => w.IsManual);
            var autoClassified = await _db.WasteItems.CountAsync(w => !w.IsManual);
            var correctAuto = await _db.WasteItems.CountAsync(w => !w.IsManual && w.IsCorrect == true);
            var correctManual = await _db.WasteItems.CountAsync(w => w.IsManual && w.IsCorrect == true);

            // Processing throughput (trend over time)
            var throughput = new List<ThroughputPoint>();
            DateTime start;
            Func<WasteItem, string> timeKey;
            if (period == "hour")
            {
                start = DateTime.UtcNow.AddHours(-24);
                timeKey = w => w.CreatedAt.ToString("yyyy-MM-dd HH:00");
            }
            else if (period == "week")
            {
                start = DateTime.UtcNow.AddDays(-28);
                timeKey = w => w.CreatedAt.ToString("yyyy-MM-dd");
            }
            else // day
            {
                start = DateTime.UtcNow.AddDays(-7);
                timeKey = w => w.CreatedAt.ToString("yyyy-MM-dd");
            }
            var items = await _db.WasteItems.Where(w => w.CreatedAt >= start).ToListAsync();
            throughput = items.GroupBy(timeKey)
                .OrderBy(g => g.Key)
                .Select(g => new ThroughputPoint { Time = g.Key, Count = g.Count() })
                .ToList();

            var summary = new StatisticSummary
            {
                Period = period,
                Accuracy = accuracy,
                VolumeByCategory = volumeByCategory,
                AlertsResolved = alertsResolved,
                AlertsUnresolved = alertsUnresolved,
                ManualClassified = manualClassified,
                AutoClassified = autoClassified,
                CorrectAuto = correctAuto,
                CorrectManual = correctManual,
                Throughput = throughput
            };
            return Ok(summary);
        }
    }
}
