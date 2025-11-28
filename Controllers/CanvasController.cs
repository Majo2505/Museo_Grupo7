using Microsoft.AspNetCore.Mvc;
using Museo.Models.Dtos.Canvas;
using Museo.Services;

namespace Museo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CanvasController : ControllerBase
    {
        private readonly ICanvasService _service;

        public CanvasController(ICanvasService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var canvas = await _service.GetById(id);
            return canvas == null ? NotFound(new { message = "Canvas not found" }) : Ok(canvas);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCanvasDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var canvas = await _service.Create(dto);
                return CreatedAtAction(nameof(GetById), new { id = canvas.Id }, canvas);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCanvasDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var updated = await _service.Update(id, dto);
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _service.Delete(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
