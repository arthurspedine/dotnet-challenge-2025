using Microsoft.AspNetCore.Mvc;
using Motoflow.Services;
using Motoflow.Models.DTOs.Motoflow.Dtos;
using Motoflow.Models;

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
            Console.WriteLine(areas);
            var areasDTO = areas.Select(AreaDTO.FromArea).ToList();
            return Ok(areasDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AreaDTO>> GetById(long id)
        {
            var area = await _areaService.GetAreaByIdAsync(id);
            if (area == null)
            {
                return NotFound();
            }
            return Ok(AreaDTO.FromArea(area));
        }

        [HttpPost]
        public async Task<ActionResult<AreaDTO>> Post(RequestAreaDTO dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }

            Area area = await _areaService.AddAreaAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = area.Id }, AreaDTO.FromArea(area));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, RequestAreaDTO dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }

            try
            {
                await _areaService.UpdateAreaAsync(id, dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(500, "Um erro ocorreu ao criar um patio.");
            }
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
