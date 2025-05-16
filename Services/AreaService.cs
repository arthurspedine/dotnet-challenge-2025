using Motoflow.Models;
using Motoflow.Repositories;
using Motoflow.Models.DTOs.Motoflow.Dtos;

namespace Motoflow.Services
{
    public class AreaService
    {
        private readonly AreaRepository _areaRepository;

        public AreaService(AreaRepository repository)
        {
            _areaRepository = repository;
        }

        public async Task<IEnumerable<AreaDTO>> GetAllAreasAsync()
        {
            var areas = await _areaRepository.GetAllAsync();
            return areas.Select(a => MapToDto(a));
        }

        public async Task<AreaDTO?> GetAreaByIdAsync(long id)
        {
            var area = await _areaRepository.GetByIdAsync(id);
            return area == null ? null : MapToDto(area);
        }

        public async Task AddAreaAsync(AreaDTO dto)
        {
            var area = MapToEntity(dto);
            await _areaRepository.AddAsync(area);
        }

        public async Task UpdateAreaAsync(AreaDTO dto)
        {
            var area = MapToEntity(dto);
            await _areaRepository.UpdateAsync(area);
        }

        public async Task DeleteAreaAsync(long id)
        {
            await _areaRepository.DeleteAsync(id);
        }

        // Map Entity to DTO
        private AreaDTO MapToDto(Area area)
        {
            return new AreaDTO
            {
                Id = area.Id,
                Identificador = area.Identificador,
                PatioId = area.PatioId,
                CapacidadeMaxima = area.CapacidadeMaxima,
                VagasDisponiveis = area.VagasDisponiveis,
                MotosIds = area.Motos.Select(m => m.Id).ToList()
            };
        }

        // Map DTO to Entity
        private Area MapToEntity(AreaDTO dto)
        {
            return new Area
            {
                Id = dto.Id,
                Identificador = dto.Identificador,
                PatioId = dto.PatioId,
                CapacidadeMaxima = dto.CapacidadeMaxima,
                

            };
        }
    }
}
