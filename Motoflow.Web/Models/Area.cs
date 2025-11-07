using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Motoflow.Web.Models
{
    public class Area
    {
        [Key]
        public long Id { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Identificador { get; set; } = string.Empty;
        
        public long PatioId { get; set; }
        public virtual Patio? Patio { get; set; }
        
        [Required]
        public int CapacidadeMaxima { get; set; }
        
        public virtual ICollection<HistoricoMoto> Historicos { get; set; } = [];

        [NotMapped]
        public IEnumerable<Moto> Motos => Historicos
            .Where(h => h.DataSaida == null)
            .Select(h => h.Moto)
            .OfType<Moto>();
        
        [NotMapped]
        public int VagasDisponiveis => CapacidadeMaxima - Motos.Count();
        
        public Area()
        {
        }

        public Area(string identificador, long patioId, int capacidadeMaxima)
        {
            Identificador = identificador ?? string.Empty;
            PatioId = patioId;
            CapacidadeMaxima = capacidadeMaxima;
        }
    }
}