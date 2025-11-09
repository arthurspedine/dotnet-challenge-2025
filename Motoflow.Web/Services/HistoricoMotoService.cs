using Motoflow.Web.Models;
using Motoflow.Web.Models.DTOs;
using Motoflow.Web.Models.Common;
using Motoflow.Web.Repositories;

namespace Motoflow.Web.Services
{
    public class HistoricoMotoService
    {
        private readonly IHistoricoMotoRepository _historicoRepository;
        private readonly MotoService _motoService;
        private readonly AreaService _areaService;

        public HistoricoMotoService(
            IHistoricoMotoRepository historicoRepository, 
            MotoService motoService,
            AreaService areaService)
        {
            _historicoRepository = historicoRepository;
            _motoService = motoService;
            _areaService = areaService;
        }

        public async Task<PagedResult<HistoricoMoto>> GetAllHistoricosAsync(PaginationQuery pagination)
        {
            return await _historicoRepository.GetAllPagedAsync(pagination);
        }

        public async Task<IEnumerable<HistoricoMoto>> GetAllHistoricosAsync()
        {
            return await _historicoRepository.GetAllAsync();
        }

        public async Task<HistoricoMoto?> GetHistoricoByIdAsync(long id)
        {
            return await _historicoRepository.GetByIdAsync(id);
        }

        public async Task<PagedResult<HistoricoMoto>> GetHistoricosByMotoIdAsync(long motoId, PaginationQuery pagination)
        {
            return await _historicoRepository.GetByMotoIdPagedAsync(motoId, pagination);
        }

        public async Task<PagedResult<HistoricoMoto>> GetHistoricosByMotoIdentifierAsync(string identifier, PaginationQuery pagination)
        {
            // Tenta buscar por ID numérico primeiro
            if (long.TryParse(identifier, out long motoId))
            {
                return await _historicoRepository.GetByMotoIdPagedAsync(motoId, pagination);
            }
            
            // Caso contrário, busca por placa, chassi ou QR code
            return await _historicoRepository.GetByMotoIdentifierPagedAsync(identifier, pagination);
        }

        public async Task<IEnumerable<HistoricoMoto>> GetHistoricosByMotoIdAsync(long motoId)
        {
            return await _historicoRepository.GetByMotoIdAsync(motoId);
        }

        public async Task<PagedResult<HistoricoMoto>> GetHistoricosByAreaIdAsync(long areaId, PaginationQuery pagination)
        {
            return await _historicoRepository.GetByAreaIdPagedAsync(areaId, pagination);
        }

        public async Task<IEnumerable<HistoricoMoto>> GetHistoricosByAreaIdAsync(long areaId)
        {
            return await _historicoRepository.GetByAreaIdAsync(areaId);
        }

        public async Task<HistoricoMoto> CreateHistoricoAsync(CreateHistoricoMotoDTO dto)
        {
            Moto moto = await _motoService.AddMotoAsync(dto.Moto);


            var area = await _areaService.GetAreaByIdAsync(dto.AreaId);
            if (area == null)
            {
                throw new KeyNotFoundException($"Área com ID {dto.AreaId} não encontrada");
            }

            var vinculoAtivo = await _historicoRepository.GetActiveByMotoIdAsync(moto.Id);
            if (vinculoAtivo != null)
            {
                throw new InvalidOperationException($"Moto já possui vínculo ativo com a área {(vinculoAtivo.Area != null ? vinculoAtivo.Area.Identificador : "desconhecida")}");
            }

            if (area.VagasDisponiveis <= 0)
            {
                throw new InvalidOperationException($"Área {area.Identificador} não possui vagas disponíveis");
            }

            var historico = new HistoricoMoto(moto.Id, dto.AreaId, dto.ObservacaoEntrada);
            await _historicoRepository.AddAsync(historico);

            return historico;
        }

        public async Task<HistoricoMoto> UpdateHistoricoAsync(long id, UpdateHistoricoMotoDTO dto)
        {
            var historico = await _historicoRepository.GetByIdAsync(id);

            if (historico == null)
            {
                throw new KeyNotFoundException($"Histórico com ID {id} não encontrado");
            }

            if (historico.DataSaida != null)
            {
                throw new InvalidOperationException("Este histórico já foi finalizado");
            }

            historico.DataSaida = dto.DataSaida ?? DateTime.Now;
            historico.ObservacaoSaida = dto.ObservacaoSaida;

            await _historicoRepository.UpdateAsync(historico);
            return historico;
        }

        public async Task<bool> DeleteHistoricoAsync(long id)
        {
            var historico = await _historicoRepository.GetByIdAsync(id);
            
            if (historico == null)
            {
                return false;
            }

            await _historicoRepository.DeleteAsync(id);
            return true;
        }
    }
}