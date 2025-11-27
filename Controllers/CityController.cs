using Microsoft.AspNetCore.Mvc;
using Museo.Models.Dtos.City;
using Museo.Services;

namespace Museo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CityController : ControllerBase
    {
        private readonly ICityService _service;

        public CityController(ICityService service)
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
            var city = await _service.GetById(id);
            return city == null ? NotFound(new { message = "City not found" }) : Ok(city);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCityDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var city = await _service.Create(dto);
                return CreatedAtAction(nameof(GetById), new { id = city.Id }, city);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCityDto dto)
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
