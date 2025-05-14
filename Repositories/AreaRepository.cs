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
    }
}
