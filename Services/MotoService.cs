using Motoflow.Models;
using Motoflow.Repositories;

namespace Motoflow.Services
{
    public class MotoService
    {
        private readonly MotoRepository _motoRepository;

        public MotoService(MotoRepository repository)
        {
            this._motoRepository = repository;
        }

        public async Task<IEnumerable<Moto>> GetAllMotosAsync()
        {
            return await _motoRepository.GetAllAsync();
        }
    }
}
