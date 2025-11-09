using Motoflow.Web.Models;
using Motoflow.Web.Models.Common;

namespace Motoflow.Web.Repositories
{
    public interface IHistoricoMotoRepository
    {
        Task<PagedResult<HistoricoMoto>> GetAllPagedAsync(PaginationQuery pagination);
        Task<IEnumerable<HistoricoMoto>> GetAllAsync();
        Task<HistoricoMoto?> GetByIdAsync(long id);
        Task<PagedResult<HistoricoMoto>> GetByMotoIdPagedAsync(long motoId, PaginationQuery pagination);
        Task<IEnumerable<HistoricoMoto>> GetByMotoIdAsync(long motoId);
        Task<PagedResult<HistoricoMoto>> GetByMotoIdentifierPagedAsync(string identifier, PaginationQuery pagination);
        Task<PagedResult<HistoricoMoto>> GetByAreaIdPagedAsync(long areaId, PaginationQuery pagination);
        Task<IEnumerable<HistoricoMoto>> GetByAreaIdAsync(long areaId);
        Task<HistoricoMoto?> GetActiveByMotoIdAsync(long motoId);
        Task<HistoricoMoto> AddAsync(HistoricoMoto historico);
        Task UpdateAsync(HistoricoMoto historico);
        Task DeleteAsync(long id);
    }
}
