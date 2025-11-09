using Motoflow.Web.Models;
using Motoflow.Web.Repositories;
using Motoflow.Web.Models.DTOs;
using Motoflow.Web.Models.Common;

namespace Motoflow.Web.Services
{
    public class AreaService
    {
        private readonly IAreaRepository _areaRepository;

        public AreaService(IAreaRepository repository)
        {
            _areaRepository = repository;
        }

        /// <summary>
        /// Obtém todas as áreas de forma paginada com HATEOAS
        /// </summary>
        public async Task<PagedResult<AreaDTO>> GetPagedAreasAsync(PaginationQuery pagination, string baseUrl)
        {
            var areas = await _areaRepository.GetAllAsync();

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

            // Adiciona links de paginação
            var hateoasLinks = new HateoasLinks();
            hateoasLinks.AddPaginationLinks($"{baseUrl}/api/v1/Area", result.Page, result.TotalPages, result.PageSize);
            result.Links = hateoasLinks.Links;

            return result;
        }

        /// <summary>
        /// Obtém uma área por ID retornando DTO com HATEOAS
        /// </summary>
        public async Task<AreaDTO?> GetAreaDTOByIdAsync(long id, string baseUrl)
        {
            var area = await _areaRepository.GetByIdAsync(id);
            return area == null ? null : AreaDTO.FromArea(area, baseUrl);
        }

        /// <summary>
        /// Cria uma nova área e retorna DTO com HATEOAS
        /// </summary>
        public async Task<AreaDTO> CreateAreaAsync(RequestAreaDTO dto, string baseUrl)
        {
            Area area = new(dto.Identificador, dto.PatioId, dto.CapacidadeMaxima);
            await _areaRepository.AddAsync(area);
            return AreaDTO.FromArea(area, baseUrl);
        }

        public async Task<IEnumerable<Area>> GetAllAreasAsync()
        {
            return await _areaRepository.GetAllAsync();
        }

        public async Task<Area?> GetAreaByIdAsync(long id)
        {
            return await _areaRepository.GetByIdAsync(id);
            
        }

        public async Task<Area> AddAreaAsync(RequestAreaDTO dto)
        {
            Area area = new(dto.Identificador, dto.PatioId, dto.CapacidadeMaxima);
            await _areaRepository.AddAsync(area);
            return area;
        }

        public async Task UpdateAreaAsync(long id, RequestAreaDTO dto)
        {
            var area = await GetAreaByIdAsync(id) ?? throw new KeyNotFoundException();
            area.Identificador = dto.Identificador;
            area.PatioId = dto.PatioId;
            area.CapacidadeMaxima = dto.CapacidadeMaxima;
            await _areaRepository.UpdateAsync(area);
        }

        public async Task<bool> DeleteAreaAsync(long id)
        {
            var area = await GetAreaByIdAsync(id);
            if (area == null)
            {
                return false;
            }

            await _areaRepository.DeleteAsync(id);
            return true;
        }
    }
}