using Motoflow.Web.Models;

namespace Motoflow.Web.Repositories
{
    public interface IAreaRepository
    {
        Task<IEnumerable<Area>> GetAllAsync();
        Task<Area?> GetByIdAsync(long id);
        Task AddAsync(Area area);
        Task UpdateAsync(Area area);
        Task DeleteAsync(long id);
    }
}
