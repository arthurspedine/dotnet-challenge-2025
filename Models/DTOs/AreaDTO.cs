using System.ComponentModel.DataAnnotations;

namespace Motoflow.Models.DTOs
{
    namespace Motoflow.Dtos
    {
        public class AreaDTO
        {
            public long Id { get; set; }

            public string Identificador { get; set; }

            public PatioDTO Patio { get; set; }

            public int CapacidadeMaxima { get; set; }

            public int VagasDisponiveis { get; set; }

            public List<long> MotosIds { get; set; } = new();

            public static AreaDTO FromArea(Area area)
            {
                return new AreaDTO
                {
                    Id = area.Id,
                    Identificador = area.Identificador,
                    Patio = PatioDTO.FromPatio(area.Patio),
                    CapacidadeMaxima = area.CapacidadeMaxima,
                    VagasDisponiveis = area.VagasDisponiveis,
                    MotosIds = area.Motos.Select(m => m.Id).ToList()
                };
            }
        }

        public class RequestAreaDTO
        {
            [Required]
            public string Identificador { get; set; }

            [Required]
            public long PatioId { get; set; }

            [Required]
            public int CapacidadeMaxima { get; set; }
        }
    }

}
