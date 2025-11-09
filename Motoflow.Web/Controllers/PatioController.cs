using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Motoflow.Web.Models;
using Motoflow.Web.Models.DTOs;
using Motoflow.Web.Models.Common;
using Motoflow.Web.Services;
using System.Net;

namespace Motoflow.Web.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de pátios
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class PatioController : ControllerBase
    {
        private readonly PatioService _patioService;

        public PatioController(PatioService patioService)
        {
            _patioService = patioService;
        }

        /// <summary>
        /// Obtém todos os pátios com paginação e HATEOAS
        /// </summary>
        /// <param name="pagination">Parâmetros de paginação</param>
        /// <returns>Lista paginada de pátios com links HATEOAS</returns>
        /// <response code="200">Retorna a lista paginada de pátios</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<PatioDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PagedResult<PatioDTO>>> Get([FromQuery] PaginationQuery pagination)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var result = await _patioService.GetPagedPatiosAsync(pagination, baseUrl);
            return Ok(result);
        }

        /// <summary>
        /// Obtém um pátio específico por ID
        /// </summary>
        /// <param name="id">ID do pátio</param>
        /// <returns>Dados do pátio com links HATEOAS</returns>
        /// <response code="200">Retorna o pátio encontrado</response>
        /// <response code="404">Pátio não encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PatioDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<PatioDTO>> GetById(long id)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var patioDTO = await _patioService.GetPatioDTOByIdAsync(id, baseUrl);
            
            if (patioDTO == null)
            {
                return NotFound();
            }
            
            return Ok(patioDTO);
        }

        /// <summary>
        /// Cria um novo pátio
        /// </summary>
        /// <param name="dto">Dados para criação do pátio</param>
        /// <returns>Pátio criado com links HATEOAS</returns>
        /// <response code="201">Pátio criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(PatioDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<PatioDTO>> Post(PatioDTO dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }

            try
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var patioDTO = await _patioService.CreatePatioAsync(dto, baseUrl);

                return CreatedAtAction(nameof(GetById), new { id = patioDTO.Id }, patioDTO);
            }
            catch (Exception)
            {
                return StatusCode(500, "Um erro ocorreu ao criar o pátio.");
            }
        }

        /// <summary>
        /// Atualiza um pátio existente
        /// </summary>
        /// <param name="id">ID do pátio a ser atualizado</param>
        /// <param name="dto">Dados de atualização do pátio</param>
        /// <returns>Status da operação</returns>
        /// <response code="204">Pátio atualizado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Pátio não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Put(long id, [FromBody] PatioDTO dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }

            try
            {
                await _patioService.UpdatePatioAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(500, "Um erro ocorreu ao atualizar o pátio.");
            }
        }

        /// <summary>
        /// Remove um pátio do sistema
        /// </summary>
        /// <param name="id">ID do pátio a ser removido</param>
        /// <returns>Status da operação</returns>
        /// <response code="204">Pátio removido com sucesso</response>
        /// <response code="404">Pátio não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var deleted = await _patioService.DeletePatioAsync(id);
                
                if (!deleted)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "Um erro ocorreu ao deletar o pátio.");
            }
        }
    }
}
