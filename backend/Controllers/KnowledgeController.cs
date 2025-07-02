using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class KnowledgeController : ControllerBase
    {
        private readonly IInferenceService _inferenceService;

        public KnowledgeController(IInferenceService inferenceService)
        {
            _inferenceService = inferenceService;
        }

        [HttpGet("infer/{wasteType}")]
        public IActionResult GetInference(string wasteType)
        {
            var result = _inferenceService.InferWasteProperties(wasteType);
            return Ok(result);
        }

        [HttpGet("categories")]
        public IActionResult GetWasteCategories()
        {
            var categories = new[]
            {
                "Battery", "Biological", "Cardboard", "Clothes", "Glass",
                "Metal", "Paper", "Plastic", "Shoes", "Trash"
            };

            var results = categories.Select(cat => new
            {
                Category = cat,
                Inference = _inferenceService.InferWasteProperties(cat)
            }).ToArray();

            return Ok(results);
        }
    }
}
