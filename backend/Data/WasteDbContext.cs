using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class WasteDbContext : DbContext
    {
        public WasteDbContext(DbContextOptions<WasteDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<WasteItem> WasteItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Alert> Alerts { get; set; }
    }
}
