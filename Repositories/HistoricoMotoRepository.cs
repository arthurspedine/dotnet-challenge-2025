using Microsoft.EntityFrameworkCore;
using Motoflow.Data;
using Motoflow.Models;

namespace Motoflow.Repositories
{
    public class HistoricoMotoRepository
    {
        private readonly OracleDbContext _context;

        public HistoricoMotoRepository(OracleDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HistoricoMoto>> GetAllAsync()
        {
            return await _context.HistoricoMotos
                .Include(h => h.Moto)
                .Include(h => h.Area)
                .ThenInclude(a => a.Patio)
                .ToListAsync();
        }

        public async Task<HistoricoMoto?> GetByIdAsync(long id)
        {
            return await _context.HistoricoMotos
                .Include(h => h.Moto)
                .Include(h => h.Area)
                    .ThenInclude(a => a.Patio)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<IEnumerable<HistoricoMoto>> GetByMotoIdAsync(long motoId)
        {
            return await _context.HistoricoMotos
                .Include(h => h.Moto)
                .Include(h => h.Area)
                    .ThenInclude(a => a.Patio)
                .Where(h => h.MotoId == motoId)
                .ToListAsync();
        }

        public async Task<IEnumerable<HistoricoMoto>> GetByAreaIdAsync(long areaId)
        {
            return await _context.HistoricoMotos
                .Include(h => h.Moto)
                .Include(h => h.Area)
                    .ThenInclude(a => a.Patio)
                .Where(h => h.AreaId == areaId)
                .ToListAsync();
        }

        public async Task<HistoricoMoto?> GetActiveByMotoIdAsync(long motoId)
        {
            return await _context.HistoricoMotos
                .Include(h => h.Moto)
                .Include(h => h.Area)
                    .ThenInclude(a => a.Patio)
                .FirstOrDefaultAsync(h => h.MotoId == motoId && h.DataSaida == null);
        }

        public async Task<HistoricoMoto> AddAsync(HistoricoMoto historico)
        {
            _context.HistoricoMotos.Add(historico);
            await _context.SaveChangesAsync();
            return historico;
        }

        public async Task UpdateAsync(HistoricoMoto historico)
        {
            _context.HistoricoMotos.Update(historico);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var historico = await _context.HistoricoMotos.FindAsync(id);
            if (historico != null)
            {
                _context.HistoricoMotos.Remove(historico);
                await _context.SaveChangesAsync();
            }
        }
    }
}