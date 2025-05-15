using Motoflow.Models;
using Motoflow.Models.DTOs;
using Motoflow.Repositories;

namespace Motoflow.Services
{
    public class PatioService
    {
        private readonly PatioRepository _patioRepository;

        public PatioService(PatioRepository repository)
        {
            _patioRepository = repository;
        }

        public async Task<IEnumerable<Patio>> GetAllPatiosAsync()
        {
            return await _patioRepository.GetAllAsync();
        }

        public async Task<Patio> GetPatioByIdAsync(long id)
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

        public async Task DeletePatioAsync(long id)
        {
            await _patioRepository.DeleteAsync(id);
        }
    }
}
