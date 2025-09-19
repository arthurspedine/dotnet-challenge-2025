using System.ComponentModel.DataAnnotations;
using Motoflow.Models.Common;

namespace Motoflow.Models.DTOs
{
    public class PatioDTO : HateoasResource
    {
        public long Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Nome { get; set; }

        [Required]
        public string Localizacao { get; set; }

        public PatioDTO(long id, string nome, string localizacao)
        {
            Id = id;
            Nome = nome;
            Localizacao = localizacao;
        }

        public PatioDTO()
        {
            Nome = string.Empty;
            Localizacao = string.Empty;
        }

        public static PatioDTO FromPatio(Patio patio, string? baseUrl = null)
        {
            var dto = new PatioDTO(patio.Id, patio.Nome, patio.Localizacao);

            // Add HATEOAS links
            if (!string.IsNullOrEmpty(baseUrl))
            {
                var hateoasLinks = new HateoasLinks();
                hateoasLinks.AddSelfLink($"{baseUrl}/api/Patio/{patio.Id}");
                hateoasLinks.AddEditLink($"{baseUrl}/api/Patio/{patio.Id}");
                hateoasLinks.AddDeleteLink($"{baseUrl}/api/Patio/{patio.Id}");
                hateoasLinks.AddCollectionLink($"{baseUrl}/api/Patio");
                hateoasLinks.AddLink("areas", $"{baseUrl}/api/Area?patioId={patio.Id}");

                dto.Links = hateoasLinks.Links;
            }

            return dto;
        }
    }
}
