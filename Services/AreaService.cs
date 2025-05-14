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
    }
}
