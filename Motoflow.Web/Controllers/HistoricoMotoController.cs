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
    /// Controller para gerenciamento do histórico de motos nas áreas
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Authorize]
    [Produces("application/json")]
    public class HistoricoMotoController : ControllerBase
    {
        private readonly HistoricoMotoService _historicoService;

        public HistoricoMotoController(HistoricoMotoService historicoService)
        {
            _historicoService = historicoService;
        }

        /// <summary>
        /// Obtém histórico de motos com paginação e HATEOAS
        /// </summary>
        /// <param name="pagination">Parâmetros de paginação</param>
        /// <returns>Lista paginada de históricos com links HATEOAS</returns>
        /// <response code="200">Retorna a lista paginada de históricos</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<HistoricoMotoDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PagedResult<HistoricoMotoDTO>>> Get([FromQuery] PaginationQuery pagination)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var historicos = await _historicoService.GetAllHistoricosAsync(pagination);
            
            var historicosDTO = historicos.Data.Select(h => HistoricoMotoDTO.FromHistoricoMoto(h, baseUrl)).ToList();
            
            var result = new PagedResult<HistoricoMotoDTO>(
                historicosDTO, 
                historicos.Page, 
                historicos.PageSize, 
                historicos.TotalCount);

            // Add pagination links
            var hateoasLinks = new HateoasLinks();
            hateoasLinks.AddPaginationLinks($"{baseUrl}/api/v2/HistoricoMoto", result.Page, result.TotalPages, result.PageSize);
            result.Links = hateoasLinks.Links;

            return Ok(result);
        }

        /// <summary>
        /// Obtém um histórico específico por ID
        /// </summary>
        /// <param name="id">ID do histórico</param>
        /// <returns>Dados do histórico com links HATEOAS</returns>
        /// <response code="200">Retorna o histórico encontrado</response>
        /// <response code="404">Histórico não encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(HistoricoMotoDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<HistoricoMotoDTO>> GetById(long id)
        {
            try
            {
                var historico = await _historicoService.GetHistoricoByIdAsync(id);
                if (historico == null)
                {
                    return NotFound();
                }

                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                return Ok(HistoricoMotoDTO.FromHistoricoMoto(historico, baseUrl));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Obtém históricos por ID da motocicleta
        /// </summary>
        /// <param name="motoId">ID da motocicleta</param>
        /// <param name="pagination">Parâmetros de paginação</param>
        /// <returns>Lista paginada de históricos da motocicleta</returns>
        /// <response code="200">Retorna os históricos da motocicleta</response>
        [HttpGet("moto/{motoId}"), MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(PagedResult<HistoricoMotoDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PagedResult<HistoricoMotoDTO>>> GetByMotoIdV1(long motoId, [FromQuery] PaginationQuery pagination)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var historicos = await _historicoService.GetHistoricosByMotoIdAsync(motoId, pagination);
            
            var historicosDTO = historicos.Data.Select(h => HistoricoMotoDTO.FromHistoricoMoto(h, baseUrl)).ToList();
            
            var result = new PagedResult<HistoricoMotoDTO>(
                historicosDTO, 
                historicos.Page, 
                historicos.PageSize, 
                historicos.TotalCount);

            var hateoasLinks = new HateoasLinks();
            hateoasLinks.AddPaginationLinks($"{baseUrl}/api/v1/HistoricoMoto/moto/{motoId}", result.Page, result.TotalPages, result.PageSize);
            result.Links = hateoasLinks.Links;

            return Ok(result);
        }

        /// <summary>
        /// Obtém históricos por identificador da motocicleta (ID, Placa, Chassi ou QR Code)
        /// </summary>
        /// <param name="moto">ID, Placa, Chassi ou QR Code da motocicleta</param>
        /// <param name="pagination">Parâmetros de paginação</param>
        /// <returns>Lista paginada de históricos da motocicleta</returns>
        /// <response code="200">Retorna os históricos da motocicleta</response>
        [HttpGet("moto/{moto}"), MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(PagedResult<HistoricoMotoDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PagedResult<HistoricoMotoDTO>>> GetByMotoIdV2(string moto, [FromQuery] PaginationQuery pagination)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var historicos = await _historicoService.GetHistoricosByMotoIdentifierAsync(moto, pagination);
            
            var historicosDTO = historicos.Data.Select(h => HistoricoMotoDTO.FromHistoricoMoto(h, baseUrl)).ToList();
            
            var result = new PagedResult<HistoricoMotoDTO>(
                historicosDTO, 
                historicos.Page, 
                historicos.PageSize, 
                historicos.TotalCount);

            var hateoasLinks = new HateoasLinks();
            hateoasLinks.AddPaginationLinks($"{baseUrl}/api/v2/HistoricoMoto/moto/{moto}", result.Page, result.TotalPages, result.PageSize);
            result.Links = hateoasLinks.Links;

            return Ok(result);
        }

        /// <summary>
        /// Obtém históricos por ID da área
        /// </summary>
        /// <param name="areaId">ID da área</param>
        /// <param name="pagination">Parâmetros de paginação</param>
        /// <returns>Lista paginada de históricos da área</returns>
        /// <response code="200">Retorna os históricos da área</response>
        [HttpGet("area/{areaId}")]
        [ProducesResponseType(typeof(PagedResult<HistoricoMotoDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PagedResult<HistoricoMotoDTO>>> GetByAreaId(long areaId, [FromQuery] PaginationQuery pagination)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var historicos = await _historicoService.GetHistoricosByAreaIdAsync(areaId, pagination);
            
            var historicosDTO = historicos.Data.Select(h => HistoricoMotoDTO.FromHistoricoMoto(h, baseUrl)).ToList();
            
            var result = new PagedResult<HistoricoMotoDTO>(
                historicosDTO, 
                historicos.Page, 
                historicos.PageSize, 
                historicos.TotalCount);

            var hateoasLinks = new HateoasLinks();
            hateoasLinks.AddPaginationLinks($"{baseUrl}/api/v2/HistoricoMoto/area/{areaId}", result.Page, result.TotalPages, result.PageSize);
            result.Links = hateoasLinks.Links;

            return Ok(result);
        }
        /// <summary>
        /// Cria um novo histórico de entrada de motocicleta em uma área
        /// </summary>
        /// <param name="dto">Dados para criação do histórico</param>
        /// <returns>Histórico criado com links HATEOAS</returns>
        /// <response code="201">Histórico criado com sucesso</response>
        /// <response code="400">Dados inválidos ou área sem vagas</response>
        /// <response code="404">Área não encontrada</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(HistoricoMotoDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<HistoricoMotoDTO>> Post(CreateHistoricoMotoDTO dto)
        {
            try
            {
                var historico = await _historicoService.CreateHistoricoAsync(dto);
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var historicoDTO = HistoricoMotoDTO.FromHistoricoMoto(historico, baseUrl);
                
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

        /// <summary>
        /// Atualiza um histórico existente (finaliza estadia com data de saída)
        /// </summary>
        /// <param name="id">ID do histórico a ser atualizado</param>
        /// <param name="dto">Dados de atualização do histórico</param>
        /// <returns>Histórico atualizado</returns>
        /// <response code="200">Histórico atualizado com sucesso</response>
        /// <response code="400">Histórico já finalizado ou dados inválidos</response>
        /// <response code="404">Histórico não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(HistoricoMotoDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<HistoricoMotoDTO>> Put(long id, UpdateHistoricoMotoDTO dto)
        {
            try
            {
                var historico = await _historicoService.UpdateHistoricoAsync(id, dto);
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                return Ok(HistoricoMotoDTO.FromHistoricoMoto(historico, baseUrl));
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

        /// <summary>
        /// Remove um histórico do sistema
        /// </summary>
        /// <param name="id">ID do histórico a ser removido</param>
        /// <returns>Status da operação</returns>
        /// <response code="204">Histórico removido com sucesso</response>
        /// <response code="404">Histórico não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var deleted = await _historicoService.DeleteHistoricoAsync(id);
                
                if (!deleted)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "Um erro ocorreu ao deletar o histórico");
            }
        }
    }
}