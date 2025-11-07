using Microsoft.EntityFrameworkCore;
using Motoflow.Web.Data;
using Motoflow.Web.Models;

namespace Motoflow.Web.Repositories
{
    public class MotoRepository
    {
        private readonly OracleDbContext _context;

        public MotoRepository(OracleDbContext context)
        {
            _context = context;
        }

        public async Task<Moto?> GetByPlacaChassiQRCodeAsync(string? placa, string? chassi, string? qrCode)
        {
            // Verificar se pelo menos um dos parâmetros não é nulo
            if (string.IsNullOrEmpty(placa) && string.IsNullOrEmpty(chassi) && string.IsNullOrEmpty(qrCode))
            {
                return null;
            }

            // Consulta base
            var query = _context.Motos
                .Include(m => m.Historicos);

            if (!string.IsNullOrEmpty(placa))
            {
                string placaLower = placa.ToLower();
                return await query.FirstOrDefaultAsync(m => m.Placa != null && m.Placa.ToLower() == placaLower);
            }

            if (!string.IsNullOrEmpty(chassi))
            {
                string chassiLower = chassi.ToLower();
                return await query.FirstOrDefaultAsync(m => m.Chassi != null && m.Chassi.ToLower() == chassiLower);
            }

            if (!string.IsNullOrEmpty(qrCode))
            {
                string qrCodeLower = qrCode.ToLower();
                return await query.FirstOrDefaultAsync(m => m.QRCode != null && m.QRCode.ToLower() == qrCodeLower);
            }

            return null;
        }

        public async Task AddAsync(Moto moto)
        {
            _context.Motos.Add(moto);
            await _context.SaveChangesAsync();
        }

        public async Task<Moto?> GetByIdAsync(long id)
        {
            return await _context.Motos
                .Include(m => m.Historicos)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
