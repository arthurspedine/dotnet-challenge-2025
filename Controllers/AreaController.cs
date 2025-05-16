using Microsoft.AspNetCore.Mvc;
using Motoflow.Services;
using Motoflow.Models.DTOs.Motoflow.Dtos;

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
        public async Task<ActionResult<IEnumerable<AreaDTO>>> Get()
        {
            var areas = await _areaService.GetAllAreasAsync();
            return Ok(areas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AreaDTO>> GetById(long id)
        {
            var area = await _areaService.GetAreaByIdAsync(id);
            if (area == null)
            {
                return NotFound();
            }
            return Ok(area);
        }

        [HttpPost]
        public async Task<ActionResult<AreaDTO>> Post(AreaDTO areaDto)
        {
            if (areaDto == null)
            {
                return BadRequest();
            }

            await _areaService.AddAreaAsync(areaDto);

            return CreatedAtAction(nameof(GetById), new { id = areaDto.Id }, areaDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, AreaDTO areaDto)
        {
            if (areaDto == null || id != areaDto.Id)
            {
                return BadRequest();
            }

            var existingArea = await _areaService.GetAreaByIdAsync(id);
            if (existingArea == null)
            {
                return NotFound();
            }

            await _areaService.UpdateAreaAsync(areaDto);

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
