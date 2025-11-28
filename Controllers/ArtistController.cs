using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Museo.Models.Dtos.Artist;
using Museo.Services;

namespace Museo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArtistController : ControllerBase
    {
        private readonly IArtistService _service;

        public ArtistController(IArtistService service)
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
            var artist = await _service.GetById(id);
            return artist == null ? NotFound(new { message = "Artist not found" }) : Ok(artist);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create([FromBody] CreateArtistDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var artist = await _service.Create(dto);
                return CreatedAtAction(nameof(GetById), new { id = artist.Id }, artist);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize (Roles = "Admin,Artista")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateArtistDto dto)
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
