using System.ComponentModel.DataAnnotations;

namespace Motoflow.Models.DTOs
{
    namespace Motoflow.Dtos
    {
        public class AreaDTO
        {
            public long Id { get; set; }

            public string Identificador { get; set; }

            public long PatioId { get; set; }

            public int CapacidadeMaxima { get; set; }

            public int VagasDisponiveis { get; set; }

            public List<long> MotosIds { get; set; } = new();
        }
    }

}
