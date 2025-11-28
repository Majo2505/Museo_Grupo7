using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Museo.Models.Dtos.Work;
using Museo.Services;

namespace Museo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkController : ControllerBase
    {
        private readonly IWorkService _service;

        public WorkController(IWorkService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{canvasId}/{artistId}")]
        public async Task<IActionResult> GetByRelation(Guid canvasId, Guid artistId)
        {
            var work = await _service.GetByRelation(canvasId, artistId);
            return work == null ? NotFound(new { message = "Relation not found" }) : Ok(work);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create([FromBody] CreateWorkDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var work = await _service.Create(dto);
                return CreatedAtAction(nameof(GetByRelation), new { canvasId = work.CanvasId, artistId = work.ArtistId }, work);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{canvasId}/{artistId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(Guid canvasId, Guid artistId)
        {
            try
            {
                await _service.Delete(canvasId, artistId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
