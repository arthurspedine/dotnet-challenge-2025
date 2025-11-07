using Motoflow.Web.Models;

namespace Motoflow.Web.Repositories
{
    public interface IPatioRepository
    {
        Task<IEnumerable<Patio>> GetAllAsync();
        Task<Patio?> GetByIdAsync(long id);
        Task AddAsync(Patio patio);
        Task UpdateAsync(Patio patio);
        Task DeleteAsync(long id);
    }
}
