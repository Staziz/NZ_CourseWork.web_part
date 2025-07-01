namespace backend.Models
{
    public class StatisticSummary
    {
        public string Period { get; set; } = string.Empty; // day, week, month
        public double Accuracy { get; set; }
        public Dictionary<string, int> VolumeByCategory { get; set; } = new();
        public int AlertsResolved { get; set; }
        public int AlertsUnresolved { get; set; }
        public int ManualClassified { get; set; }
        public int AutoClassified { get; set; }
        public int CorrectAuto { get; set; }
        public int CorrectManual { get; set; }
        public List<ThroughputPoint> Throughput { get; set; } = new();
    }

    public class ThroughputPoint
    {
        public string Time { get; set; } = string.Empty; // e.g. hour, day, week
        public int Count { get; set; }
    }
}
