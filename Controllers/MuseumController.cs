using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Museo.Models.Dtos.Museum;
using Museo.Services;

namespace Museo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MuseumController : ControllerBase
    {
        private readonly IMuseumService _service;

        public MuseumController(IMuseumService service)
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
            var museum = await _service.GetById(id);
            return museum == null ? NotFound(new { message = "Museum not found" }) : Ok(museum);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create([FromBody] CreateMuseumDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var museum = await _service.Create(dto);
                return CreatedAtAction(nameof(GetById), new { id = museum.Id }, museum);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMuseumDto dto)
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
        [Authorize(Policy = "AdminOnly")]
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
