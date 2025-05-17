using Motoflow.Models;
using Motoflow.Repositories;
using Motoflow.Models.DTOs.Motoflow.Dtos;
using Motoflow.Models.DTOs;

namespace Motoflow.Services
{
    public class AreaService
    {
        private readonly AreaRepository _areaRepository;

        public AreaService(AreaRepository repository)
        {
            _areaRepository = repository;
        }

        public async Task<IEnumerable<Area>> GetAllAreasAsync()
        {
            return await _areaRepository.GetAllAsync();
        }

        public async Task<Area> GetAreaByIdAsync(long id)
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

        public async Task DeleteAreaAsync(long id)
        {
            await _areaRepository.DeleteAsync(id);
        }
    }
}
