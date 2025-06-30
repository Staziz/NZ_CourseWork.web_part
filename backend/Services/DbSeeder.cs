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

            db.SaveChanges();
        }
    }
}
