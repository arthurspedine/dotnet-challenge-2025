using Microsoft.EntityFrameworkCore;
using Motoflow.Data;
using Motoflow.Models;

namespace Motoflow.Repositories
{
    public class AreaRepository
    {
        private readonly OracleDbContext _context;

        public AreaRepository(OracleDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Area>> GetAllAsync()
        {
            return await _context.Areas.ToListAsync();
        }

        public async Task<Area> GetByIdAsync(long id)
        {
            return await _context.Areas.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAsync(Area area)
        {
            await _context.Areas.AddAsync(area);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Area area)
        {
            _context.Areas.Update(area);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var area = await _context.Areas.FindAsync(id);
            if (area != null)
            {
                _context.Areas.Remove(area);
                await _context.SaveChangesAsync();
            }
        }
    }
}
