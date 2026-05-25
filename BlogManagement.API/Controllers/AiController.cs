using BlogManagement.API.Models.Common;
using BlogManagement.API.Models.DTOs.Ai;
using BlogManagement.API.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AiController : ControllerBase
    {
        private readonly IAiService _aiService;

        public AiController(IAiService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("enhance")]
        public async Task<IActionResult> Enhance([FromBody] AiEnhanceRequest request)
        {
            var enhanced = await _aiService.EnhanceContentAsync(request);
            return Ok(ApiResponse<string>.Ok(enhanced, "Content enhanced successfully."));
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] AiChatRequest request)
        {
            var response = await _aiService.ChatAsync(request);
            return Ok(ApiResponse<string>.Ok(response));
        }
    }
}
