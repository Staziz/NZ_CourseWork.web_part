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
            db.Database.Migrate();

            // Seed users
            if (!db.Users.Any())
            {
                db.Users.AddRange(
                    new User { Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), Role = "admin" },
                    new User { Username = "service1", PasswordHash = BCrypt.Net.BCrypt.HashPassword("service123"), Role = "service_personnel" }
                );
            }

            // Seed categories
            if (!db.Categories.Any())
            {
                db.Categories.AddRange(
                    new Category { Name = "Plastic" },
                    new Category { Name = "Glass" },
                    new Category { Name = "Metal" },
                    new Category { Name = "Paper" },
                    new Category { Name = "Battery" },
                    new Category { Name = "Clothes" }
                );
            }

            db.SaveChanges();
        }
    }
}
