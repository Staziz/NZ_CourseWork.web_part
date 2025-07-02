namespace backend.Models
{
    public class WasteInferenceResult
    {
        public string WasteType { get; set; } = string.Empty;
        public bool IsHazardous { get; set; }
        public bool IsRecyclable { get; set; }
        public string Conveyor { get; set; } = string.Empty;
        public List<string> Warnings { get; set; } = new();
        public string RecyclingInfo { get; set; } = string.Empty;
    }
}
