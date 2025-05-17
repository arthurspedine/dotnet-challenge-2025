using System;
using System.ComponentModel.DataAnnotations;
using Motoflow.Models;
using Motoflow.Models.DTOs.Motoflow.Dtos;

namespace Motoflow.Models.DTOs
{
    public class HistoricoMotoDTO
    {
        public long Id { get; set; }
        public MotoDTO? Moto { get; set; }
        public AreaDTO? Area { get; set; }
        public DateTime DataEntrada { get; set; }
        public DateTime? DataSaida { get; set; }
        public string ObservacaoEntrada { get; set; } = string.Empty;
        public string? ObservacaoSaida { get; set; }
        public bool Ativo { get; set; }

        public static HistoricoMotoDTO FromHistoricoMoto(HistoricoMoto historico)
        {
            if (historico == null)
                return new HistoricoMotoDTO();

            return new HistoricoMotoDTO
            {
                Id = historico.Id,
                Moto = historico.Moto != null ? MotoDTO.FromMoto(historico.Moto) : null,
                Area = historico.Area != null ? AreaDTO.FromArea(historico.Area) : null,
                DataEntrada = historico.DataEntrada,
                DataSaida = historico.DataSaida,
                ObservacaoEntrada = historico.ObservacaoEntrada ?? string.Empty,
                ObservacaoSaida = historico.ObservacaoSaida,
                Ativo = historico.Ativo
            };
        }
    }

    public class CreateHistoricoMotoDTO
    {
        [Required]
        public CreateMotoDTO Moto { get; set; } = new();
        [Required]
        public long AreaId { get; set; }
        [Required]
        public string ObservacaoEntrada { get; set; } = string.Empty;
    }

    public class UpdateHistoricoMotoDTO
    {
        public DateTime? DataSaida { get; set; } = DateTime.Now;
        public string ObservacaoSaida { get; set; } = string.Empty;
    }
}