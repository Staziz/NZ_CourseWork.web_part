using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using backend.Models;

namespace backend.Services
{
    public interface IInferenceService
    {
        WasteInferenceResult InferWasteProperties(string wasteType);
    }

    public class InferenceService : IInferenceService
    {
        private readonly IGraph _knowledgeBase;
        private readonly string _namespace = "http://example.org/waste#";

        public InferenceService(IWebHostEnvironment env)
        {
            _knowledgeBase = new Graph();
            var ttlPath = Path.Combine(env.ContentRootPath, "Data", "eco_kb_txt.ttl");
            
            if (File.Exists(ttlPath))
            {
                var parser = new TurtleParser();
                parser.Load(_knowledgeBase, ttlPath);
            }
        }

        public WasteInferenceResult InferWasteProperties(string wasteType)
        {
            var result = new WasteInferenceResult
            {
                WasteType = wasteType,
                IsHazardous = false,
                IsRecyclable = false,
                Warnings = new List<string>(),
                RecyclingInfo = "General waste disposal"
            };

            try
            {
                // Create URI for the waste type
                var wasteTypeUri = _knowledgeBase.CreateUriNode(UriFactory.Create($"{_namespace}{wasteType}"));
                var hazardousUri = _knowledgeBase.CreateUriNode(UriFactory.Create($"{_namespace}Hazardous"));
                var recyclableUri = _knowledgeBase.CreateUriNode(UriFactory.Create($"{_namespace}Recyclable"));
                var subClassOfUri = _knowledgeBase.CreateUriNode(UriFactory.Create("http://www.w3.org/2000/01/rdf-schema#subClassOf"));

                // Check if waste type is hazardous
                var hazardousTriples = _knowledgeBase.GetTriplesWithSubjectPredicate(wasteTypeUri, subClassOfUri)
                    .Where(t => t.Object.Equals(hazardousUri));
                
                if (hazardousTriples.Any())
                {
                    result.IsHazardous = true;
                    result.Warnings.Add("⚠️ Hazardous material detected - requires special handling");
                    result.RecyclingInfo = "Hazardous waste - must be disposed of at specialized facility";
                }

                // Check if waste type is recyclable
                var recyclableTriples = _knowledgeBase.GetTriplesWithSubjectPredicate(wasteTypeUri, subClassOfUri)
                    .Where(t => t.Object.Equals(recyclableUri));

                if (recyclableTriples.Any())
                {
                    result.IsRecyclable = true;
                    result.RecyclingInfo = "Recyclable material - can be processed for reuse";
                }

                // Determine conveyor
                result.Conveyor = DetermineConveyor(wasteType);

                // Add specific warnings based on waste type
                AddSpecificWarnings(wasteType, result);

            }
            catch (Exception ex)
            {
                result.Warnings.Add($"Error during inference: {ex.Message}");
            }

            return result;
        }

        private string DetermineConveyor(string wasteType)
        {
            return wasteType.ToLower() switch
            {
                "battery" => "ConveyorBatteries",
                "biological" => "ConveyorOrganic",
                "cardboard" => "ConveyorCardboard",
                "clothes" => "ConveyorClothes",
                "glass" => "ConveyorGlass",
                "metal" => "ConveyorMetal",
                "paper" => "ConveyorPaper",
                "plastic" => "ConveyorPlastic",
                "shoes" => "ConveyorShoes",
                "trash" => "ConveyorTrash",
                _ => "ConveyorTrash"
            };
        }

        private void AddSpecificWarnings(string wasteType, WasteInferenceResult result)
        {
            switch (wasteType.ToLower())
            {
                case "battery":
                    result.Warnings.Add("🔋 High voltage risk - handle with care");
                    break;
                case "biological":
                    result.Warnings.Add("🦠 Organic material - may contain contaminants");
                    break;
                case "glass":
                    result.Warnings.Add("🗂️ Fragile material - risk of breakage");
                    break;
                case "clothes":
                    if (DateTime.Now.Hour > 12) // Mock condition for wet clothes
                    {
                        result.Warnings.Add("💧 Possible moisture detected");
                    }
                    break;
            }
        }
    }
}
