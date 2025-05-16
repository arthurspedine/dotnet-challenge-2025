using Microsoft.AspNetCore.Mvc;
using Motoflow.Models;
using Motoflow.Models.DTOs;
using Motoflow.Services;

namespace Motoflow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatioController : ControllerBase
    {
        private readonly PatioService _patioService;

        public PatioController(PatioService patioService)
        {
            _patioService = patioService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatioDTO>>> Get()
        {
            var patios = await _patioService.GetAllPatiosAsync();
            var patiosDTO = patios.Select(PatioDTO.FromPatio).ToList();
            return Ok(patiosDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatioDTO>> GetById(long id)
        {
            var patio = await _patioService.GetPatioByIdAsync(id);
            if (patio == null)
            {
                return NotFound();
            }
            return Ok(PatioDTO.FromPatio(patio));
        }

        [HttpPost]
        public async Task<ActionResult<PatioDTO>> Post(PatioDTO dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }

            Patio patio = await _patioService.AddPatioAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = patio.Id }, PatioDTO.FromPatio(patio));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] PatioDTO dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }

            try
            {
                await _patioService.UpdatePatioAsync(id, dto);
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
            var patio = await _patioService.GetPatioByIdAsync(id);
            if (patio == null)
            {
                return NotFound();
            }

            await _patioService.DeletePatioAsync(id);

            return NoContent();
        }
    }
}
