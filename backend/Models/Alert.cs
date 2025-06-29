using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Alert
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty; // e.g., contamination
        public string Description { get; set; } = string.Empty;
        public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
        public string? AssignedTo { get; set; }
        public bool Closed { get; set; } = false;
    }
}
