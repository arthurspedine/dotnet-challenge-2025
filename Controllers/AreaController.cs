using Microsoft.AspNetCore.Mvc;
using Motoflow.Models;
using Motoflow.Services;

namespace Motoflow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : ControllerBase
    {

        private readonly AreaService _areaService;
        
        public AreaController(AreaService areaService)
        {
            _areaService = areaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Area>>> Get()
        {
            return Ok(await _areaService.GetAllAreasAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Area>> GetById(long id)
        {
            var area = await _areaService.GetAreaByIdAsync(id);
            if (area == null)
            {
                return NotFound();
            }
            return Ok(area);
        }

        [HttpPost]
        public async Task<ActionResult<Area>> Post(Area area)
        {
            if (area == null)
            {
                return BadRequest();
            }

            await _areaService.AddAreaAsync(area);

            return CreatedAtAction(nameof(GetById), new { id = area.Id }, area);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, Area area)
        {
            if (area == null || id != area.Id)
            {
                return BadRequest();
            }

            var existingArea = await _areaService.GetAreaByIdAsync(id);
            if (existingArea == null)
            {
                return NotFound();
            }

            await _areaService.UpdateAreaAsync(area);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var area = await _areaService.GetAreaByIdAsync(id);
            if (area == null)
            {
                return NotFound();
            }

            await _areaService.DeleteAreaAsync(id);

            return NoContent();
        }
    }
}
