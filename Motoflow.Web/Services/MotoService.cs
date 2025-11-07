using Motoflow.Web.Models;
using Motoflow.Web.Models.DTOs;
using Motoflow.Web.Repositories;

namespace Motoflow.Web.Services
{
    public class MotoService
    {
        private readonly MotoRepository _motoRepository;

        public MotoService(MotoRepository repository)
        {
            this._motoRepository = repository;
        }

        public async Task<Moto> AddMotoAsync(CreateMotoDTO dto)
        {
            Moto? moto = await GetMotoByPlacaChassiQRCodeAsync(dto.Placa, dto.Chassi, dto.QRCode);
            if (moto == null) {
                moto = new(dto.Type, dto.Placa, dto.Chassi, dto.QRCode);
                await _motoRepository.AddAsync(moto);
            }
            var result = await GetMotoByIdAsync(moto.Id);
            if (result == null)
            {
                throw new InvalidOperationException("Moto não encontrada após criação.");
            }
            return result;
        }

        public async Task<Moto?> GetMotoByIdAsync(long id)
        {
            return await _motoRepository.GetByIdAsync(id);
        }

        public async Task<Moto?> GetMotoByPlacaChassiQRCodeAsync(string? placa, string? chassi, string? qrCode)
        {
            return await _motoRepository.GetByPlacaChassiQRCodeAsync(placa ?? string.Empty, chassi ?? string.Empty, qrCode ?? string.Empty);
        }
    }
}
