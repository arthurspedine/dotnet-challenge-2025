using System.ComponentModel.DataAnnotations;

namespace Motoflow.Web.Models
{
    public class Patio
    {
        [Key]
        public long Id { get; set; }
        
        [Required]
        [StringLength(150)]
        public string Nome { get; set; }
        
        [Required]
        public string Localizacao { get; set; }
        
        // Áreas deste pátio
        public virtual ICollection<Area> Areas { get; set; } = new List<Area>();

        public Patio(string nome, string localizacao) {
            Nome = nome;
            Localizacao = localizacao;
        }
    } 
}