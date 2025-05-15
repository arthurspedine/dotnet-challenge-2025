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
    }
}
