using Motoflow.Models;
using Motoflow.Repositories;

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

        public async Task AddAreaAsync(Area area)
        {
            await _areaRepository.AddAsync(area);
        }

        public async Task UpdateAreaAsync(Area area)
        {
            await _areaRepository.UpdateAsync(area);
        }

        public async Task DeleteAreaAsync(long id)
        {
            await _areaRepository.DeleteAsync(id);
        }
    }
}
