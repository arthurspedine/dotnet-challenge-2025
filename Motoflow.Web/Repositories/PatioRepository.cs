using Microsoft.EntityFrameworkCore;
using Motoflow.Web.Data;
using Motoflow.Web.Models;

namespace Motoflow.Web.Repositories
{
    public class PatioRepository
    {
        private readonly OracleDbContext _context;

        public PatioRepository(OracleDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Patio>> GetAllAsync()
        {
            return await _context.Patios.Include(p => p.Areas).ToListAsync();
        }

        public async Task<Patio?> GetByIdAsync(long id)
        {
            return await _context.Patios
                .Include(p => p.Areas)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Patio patio)
        {
            await _context.Patios.AddAsync(patio);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Patio patio)
        {
            _context.Patios.Update(patio);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var patio = await _context.Patios.FindAsync(id);
            if (patio != null)
            {
                _context.Patios.Remove(patio);
                await _context.SaveChangesAsync();
            }
        }
    }
}
