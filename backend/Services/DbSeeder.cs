using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public static class DbSeeder
    {
        public static void Seed(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<WasteDbContext>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            db.Database.Migrate();

            // Seed users
            if (!db.Users.Any())
            {
                db.Users.AddRange(
                    new User { Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), Role = "admin" },
                    new User { Username = "service1", PasswordHash = BCrypt.Net.BCrypt.HashPassword("service123"), Role = "service_personnel" }
                );
            }

            // Seed categories from config
            if (!db.Categories.Any())
            {
                var categories = config.GetSection("WasteCategories").Get<string[]>();
                if (categories != null)
                {
                    db.Categories.AddRange(categories.Select(name => new Category { Name = name }));
                }
            }

            // Seed demo alerts
            if (!db.Alerts.Any())
            {
                db.Alerts.AddRange(
                    new Alert { Type = "contamination", Description = "Battery contamination detected in organic waste bin", DetectedAt = DateTime.UtcNow.AddHours(-2), Closed = false },
                    new Alert { Type = "contamination", Description = "Glass fragments found in plastic conveyor", DetectedAt = DateTime.UtcNow.AddHours(-1), Closed = true, AssignedTo = "service1" },
                    new Alert { Type = "system", Description = "High moisture level detected in clothes sorting area", DetectedAt = DateTime.UtcNow.AddMinutes(-30), Closed = false }
                );
            }

            db.SaveChanges();
        }
    }
}
