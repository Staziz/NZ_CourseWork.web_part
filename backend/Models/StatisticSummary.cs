namespace backend.Models
{
    public class StatisticSummary
    {
        public string Period { get; set; } = string.Empty; // day, week, month
        public double Accuracy { get; set; }
        public Dictionary<string, int> VolumeByCategory { get; set; } = new();
    }
}
