using Microsoft.AspNetCore.Mvc;
using Motoflow.Models;
using Motoflow.Models.DTOs;
using Motoflow.Services;

namespace Motoflow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoricoMotoController : ControllerBase
    {
        private readonly HistoricoMotoService _historicoService;

        public HistoricoMotoController(HistoricoMotoService historicoService)
        {
            _historicoService = historicoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HistoricoMotoDTO>>> Get()
        {
            var historicos = await _historicoService.GetAllHistoricosAsync();
            var historicosDTO = historicos.Select(HistoricoMotoDTO.FromHistoricoMoto).ToList();
            return Ok(historicosDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HistoricoMotoDTO>> GetById(long id)
        {
            try
            {
                var historico = await _historicoService.GetHistoricoByIdAsync(id);
                if (historico == null)
                {
                    return NotFound();
                }
                return Ok(HistoricoMotoDTO.FromHistoricoMoto(historico));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("moto/{motoId}")]
        public async Task<ActionResult<IEnumerable<HistoricoMotoDTO>>> GetByMotoId(long motoId)
        {
            var historicos = await _historicoService.GetHistoricosByMotoIdAsync(motoId);
            var historicosDTO = historicos.Select(HistoricoMotoDTO.FromHistoricoMoto).ToList();
            return Ok(historicosDTO);
        }

        [HttpGet("area/{areaId}")]
        public async Task<ActionResult<IEnumerable<HistoricoMotoDTO>>> GetByAreaId(long areaId)
        {
            var historicos = await _historicoService.GetHistoricosByAreaIdAsync(areaId);
            var historicosDTO = historicos.Select(HistoricoMotoDTO.FromHistoricoMoto).ToList();
            return Ok(historicosDTO);
        }

        [HttpPost]
        public async Task<ActionResult<HistoricoMotoDTO>> Post(CreateHistoricoMotoDTO dto)
        {
            try
            {
                var historico = await _historicoService.CreateHistoricoAsync(dto);
                var historicoDTO = HistoricoMotoDTO.FromHistoricoMoto(historico);
                
                return CreatedAtAction(
                    nameof(GetById), 
                    new { id = historico.Id }, 
                    historicoDTO);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Um erro ocorreu ao criar o histórico");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<HistoricoMotoDTO>> Put(long id, UpdateHistoricoMotoDTO dto)
        {
            try
            {
                var historico = await _historicoService.UpdateHistoricoAsync(id, dto);
                return Ok(HistoricoMotoDTO.FromHistoricoMoto(historico));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Um erro ocorreu ao atualizar o histórico");
            }
        }
    }
}