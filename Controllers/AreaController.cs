﻿using Microsoft.AspNetCore.Mvc;
using Motoflow.Services;
using Motoflow.Models.DTOs.Motoflow.Dtos;
using Motoflow.Models.Common;
using Motoflow.Models;
using System.Net;

namespace Motoflow.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de áreas dentro dos pátios
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AreaController : ControllerBase
    {
        private readonly AreaService _areaService;

        public AreaController(AreaService areaService)
        {
            _areaService = areaService;
        }

        /// <summary>
        /// Obtém todas as áreas com paginação e HATEOAS
        /// </summary>
        /// <param name="pagination">Parâmetros de paginação</param>
        /// <returns>Lista paginada de áreas com links HATEOAS</returns>
        /// <response code="200">Retorna a lista paginada de áreas</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<AreaDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PagedResult<AreaDTO>>> Get([FromQuery] PaginationQuery pagination)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var areas = await _areaService.GetAllAreasAsync();

            var totalCount = areas.Count();
            var pagedAreas = areas
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize);

            var areasDTO = pagedAreas.Select(a => AreaDTO.FromArea(a, baseUrl)).ToList();
            
            var result = new PagedResult<AreaDTO>(
                areasDTO, 
                pagination.Page, 
                pagination.PageSize, 
                totalCount);

            var hateoasLinks = new HateoasLinks();
            hateoasLinks.AddPaginationLinks($"{baseUrl}/api/Area", result.Page, result.TotalPages, result.PageSize);
            result.Links = hateoasLinks.Links;

            return Ok(result);
        }

        /// <summary>
        /// Obtém uma área específica por ID
        /// </summary>
        /// <param name="id">ID da área</param>
        /// <returns>Dados da área com links HATEOAS</returns>
        /// <response code="200">Retorna a área encontrada</response>
        /// <response code="404">Área não encontrada</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AreaDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<AreaDTO>> GetById(long id)
        {
            var area = await _areaService.GetAreaByIdAsync(id);
            if (area == null)
            {
                return NotFound();
            }
            
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            return Ok(AreaDTO.FromArea(area, baseUrl));
        }

        /// <summary>
        /// Cria uma nova área em um pátio
        /// </summary>
        /// <param name="dto">Dados para criação da área</param>
        /// <returns>Área criada com links HATEOAS</returns>
        /// <response code="201">Área criada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(AreaDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<AreaDTO>> Post(RequestAreaDTO dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }

            try
            {
                Area area = await _areaService.AddAreaAsync(dto);
                var baseUrl = $"{Request.Scheme}://{Request.Host}";

                return CreatedAtAction(nameof(GetById), new { id = area.Id }, AreaDTO.FromArea(area, baseUrl));
            }
            catch (Exception)
            {
                return StatusCode(500, "Um erro ocorreu ao criar a área.");
            }
        }

        /// <summary>
        /// Atualiza uma área existente
        /// </summary>
        /// <param name="id">ID da área a ser atualizada</param>
        /// <param name="dto">Dados de atualização da área</param>
        /// <returns>Status da operação</returns>
        /// <response code="204">Área atualizada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Área não encontrada</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Put(long id, RequestAreaDTO dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }

            try
            {
                await _areaService.UpdateAreaAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(500, "Um erro ocorreu ao atualizar a área.");
            }
        }

        /// <summary>
        /// Remove uma área do sistema
        /// </summary>
        /// <param name="id">ID da área a ser removida</param>
        /// <returns>Status da operação</returns>
        /// <response code="204">Área removida com sucesso</response>
        /// <response code="404">Área não encontrada</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var area = await _areaService.GetAreaByIdAsync(id);
                if (area == null)
                {
                    return NotFound();
                }

                await _areaService.DeleteAreaAsync(id);
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "Um erro ocorreu ao deletar a área.");
            }
        }
    }
}
