using System.ComponentModel.DataAnnotations;

namespace Motoflow.Models.DTOs
{
    public class PatioDTO
    {
        public long Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Nome { get; set; }

        [Required]
        public string Localizacao { get; set; }

        public PatioDTO(long id, string nome, string localizacao) {
            Id = id;
            Nome = nome;
            Localizacao = localizacao;
        }
    }
}
