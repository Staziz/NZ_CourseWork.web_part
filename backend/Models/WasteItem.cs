using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class WasteItem
    {
        [Key]
        public int Id { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string Status { get; set; } = "pending"; // pending, classified, unclassified
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? Result { get; set; }
    }
}
