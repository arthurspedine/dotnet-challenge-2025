using System.ComponentModel.DataAnnotations;
using Motoflow.Web.Models.Common;

namespace Motoflow.Web.Models.DTOs
{
    public class AreaDTO : HateoasResource
        {
            public long Id { get; set; }
            public string Identificador { get; set; } = string.Empty;
            public PatioDTO? Patio { get; set; }
            public int CapacidadeMaxima { get; set; }
            public int VagasDisponiveis { get; set; }
            public List<long> MotosIds { get; set; } = [];

            public static AreaDTO FromArea(Area area, string? baseUrl = null)
            {
                if (area == null)
                    return new AreaDTO();

                var dto = new AreaDTO
                {
                    Id = area.Id,
                    Identificador = area.Identificador,
                    Patio = area.Patio != null ? PatioDTO.FromPatio(area.Patio, baseUrl) : null,
                    CapacidadeMaxima = area.CapacidadeMaxima,
                    VagasDisponiveis = area.VagasDisponiveis,
                    MotosIds = area.Motos?.Select(m => m.Id).ToList() ?? []
                };

                // Add HATEOAS links
                if (!string.IsNullOrEmpty(baseUrl))
                {
                    var hateoasLinks = new HateoasLinks();
                    hateoasLinks.AddSelfLink($"{baseUrl}/api/Area/{area.Id}");
                    hateoasLinks.AddEditLink($"{baseUrl}/api/Area/{area.Id}");
                    hateoasLinks.AddDeleteLink($"{baseUrl}/api/Area/{area.Id}");
                    hateoasLinks.AddCollectionLink($"{baseUrl}/api/Area");
                    
                    if (area.Patio != null)
                    {
                        hateoasLinks.AddLink("patio", $"{baseUrl}/api/Patio/{area.Patio.Id}");
                    }
                    
                    hateoasLinks.AddLink("historicos", $"{baseUrl}/api/HistoricoMoto/area/{area.Id}");

                    dto.Links = hateoasLinks.Links;
                }

                return dto;
            }
        }

        public class RequestAreaDTO
        {
            [Required]
            public string Identificador { get; set; } = string.Empty;
            [Required]
            public long PatioId { get; set; }
            [Required]
            public int CapacidadeMaxima { get; set; }
        }
}
