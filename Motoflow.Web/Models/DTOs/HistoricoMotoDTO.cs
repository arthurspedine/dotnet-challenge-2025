using System;
using System.ComponentModel.DataAnnotations;
using Motoflow.Web.Models;
using Motoflow.Web.Models.DTOs;
using Motoflow.Web.Models.Common;

namespace Motoflow.Web.Models.DTOs
{
    public class HistoricoMotoDTO : HateoasResource
    {
        public long Id { get; set; }
        public MotoDTO? Moto { get; set; }
        public AreaDTO? Area { get; set; }
        public DateTime DataEntrada { get; set; }
        public DateTime? DataSaida { get; set; }
        public string ObservacaoEntrada { get; set; } = string.Empty;
        public string? ObservacaoSaida { get; set; }
        public bool Ativo { get; set; }

        public static HistoricoMotoDTO FromHistoricoMoto(HistoricoMoto historico, string? baseUrl = null)
        {
            if (historico == null)
                return new HistoricoMotoDTO();

            var dto = new HistoricoMotoDTO
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

            // Add HATEOAS links
            if (!string.IsNullOrEmpty(baseUrl))
            {
                var hateoasLinks = new HateoasLinks();
                hateoasLinks.AddSelfLink($"{baseUrl}/api/HistoricoMoto/{historico.Id}");
                hateoasLinks.AddEditLink($"{baseUrl}/api/HistoricoMoto/{historico.Id}");
                hateoasLinks.AddDeleteLink($"{baseUrl}/api/HistoricoMoto/{historico.Id}");
                hateoasLinks.AddCollectionLink($"{baseUrl}/api/HistoricoMoto");
                
                if (historico.Moto != null)
                {
                    hateoasLinks.AddLink("moto", $"{baseUrl}/api/Moto/{historico.Moto.Id}");
                }
                
                if (historico.Area != null)
                {
                    hateoasLinks.AddLink("area", $"{baseUrl}/api/Area/{historico.Area.Id}");
                }

                dto.Links = hateoasLinks.Links;
            }

            return dto;
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