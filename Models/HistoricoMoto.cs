using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Motoflow.Models {
    public class HistoricoMoto
    {
        [Key]
        public long Id { get; set; }
        
        [Required]
        public long MotoId { get; set; }
        
        public virtual Moto Moto { get; set; }
        
        [Required]
        public long AreaId { get; set; }
        
        public virtual Area Area { get; set; }
        
        [Required]
        public DateTime DataEntrada { get; set; }
        
        public DateTime? DataSaida { get; set; }
        
        [Required(ErrorMessage = "A observação de entrada é obrigatória.")]
        [StringLength(500, ErrorMessage = "A observação de entrada deve ter no máximo 500 caracteres.")]
        public string ObservacaoEntrada { get; set; }

        [StringLength(500, ErrorMessage = "A observação de saída deve ter no máximo 500 caracteres.")]
        public string? ObservacaoSaida { get; set; }

        // Indica se o histórico ainda está ativo
        [NotMapped]
        public bool Ativo => DataSaida == null;

        public HistoricoMoto(long motoId, long areaId, string observacaoEntrada)
        {
            MotoId = motoId;
            AreaId = areaId;
            ObservacaoEntrada = observacaoEntrada;
            DataEntrada = DateTime.Now;
        }
    }
}