using Motoflow.Web.Models;
using Motoflow.Web.Models.DTOs;
using Motoflow.Web.Repositories;
using Motoflow.Web.Models.Common;

namespace Motoflow.Web.Services
{
    public class PatioService
    {
        private readonly IPatioRepository _patioRepository;

        public PatioService(IPatioRepository repository)
        {
            _patioRepository = repository;
        }

        /// <summary>
        /// Obtém todos os pátios de forma paginada com HATEOAS
        /// </summary>
        public async Task<PagedResult<PatioDTO>> GetPagedPatiosAsync(PaginationQuery pagination, string baseUrl)
        {
            var patios = await _patioRepository.GetAllAsync();

            var totalCount = patios.Count();
            var pagedPatios = patios
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize);

            var patiosDTO = pagedPatios.Select(p => PatioDTO.FromPatio(p, baseUrl)).ToList();
            
            var result = new PagedResult<PatioDTO>(
                patiosDTO, 
                pagination.Page, 
                pagination.PageSize, 
                totalCount);

            // Adiciona links de paginação
            var hateoasLinks = new HateoasLinks();
            hateoasLinks.AddPaginationLinks($"{baseUrl}/api/v1/Patio", result.Page, result.TotalPages, result.PageSize);
            result.Links = hateoasLinks.Links;

            return result;
        }

        /// <summary>
        /// Obtém um pátio por ID retornando DTO com HATEOAS
        /// </summary>
        public async Task<PatioDTO?> GetPatioDTOByIdAsync(long id, string baseUrl)
        {
            var patio = await _patioRepository.GetByIdAsync(id);
            return patio == null ? null : PatioDTO.FromPatio(patio, baseUrl);
        }

        /// <summary>
        /// Cria um novo pátio e retorna DTO com HATEOAS
        /// </summary>
        public async Task<PatioDTO> CreatePatioAsync(PatioDTO dto, string baseUrl)
        {
            Patio patio = new Patio(dto.Nome, dto.Localizacao);
            await _patioRepository.AddAsync(patio);
            return PatioDTO.FromPatio(patio, baseUrl);
        }

        public async Task<IEnumerable<Patio>> GetAllPatiosAsync()
        {
            return await _patioRepository.GetAllAsync();
        }

        public async Task<Patio?> GetPatioByIdAsync(long id)
        {
            return await _patioRepository.GetByIdAsync(id);
        }

        public async Task<Patio> AddPatioAsync(PatioDTO dto)
        {
            Patio patio = new Patio(dto.Nome, dto.Localizacao);
            await _patioRepository.AddAsync(patio);
            return patio;
        }

        public async Task UpdatePatioAsync(long id, PatioDTO dto)
        {
            var patio = await GetPatioByIdAsync(id) ?? throw new KeyNotFoundException();
            patio.Nome = dto.Nome;
            patio.Localizacao = dto.Localizacao;
            await _patioRepository.UpdateAsync(patio);
        }

        public async Task<bool> DeletePatioAsync(long id)
        {
            var patio = await GetPatioByIdAsync(id);
            if (patio == null)
            {
                return false;
            }

            await _patioRepository.DeleteAsync(id);
            return true;
        }
    }
}