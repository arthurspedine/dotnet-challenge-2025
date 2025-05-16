using Microsoft.EntityFrameworkCore;
using Motoflow.Data;
using Motoflow.Models;

namespace Motoflow.Repositories
{
    public class MotoRepository
    {
        private readonly OracleDbContext _context;

        public MotoRepository(OracleDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Moto>> GetAllAsync()
        {
            return await _context.Motos.ToListAsync();
        }

        public async Task<Moto?> GetByIdAsync(long id)
        {
            return await _context.Motos
                .Include(m => m.Historicos) // Inclui relacionamentos, se necessário
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddAsync(Moto moto)
        {
            _context.Motos.Add(moto);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Moto moto)
        {
            _context.Motos.Update(moto);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto != null)
            {
                _context.Motos.Remove(moto);
                await _context.SaveChangesAsync();
            }
        }
    }
}
