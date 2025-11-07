using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Motoflow.Web.Models
{
    public class Moto
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public MotoType Type { get; set; }

        // Campos de identificação (pelo menos um deve ser preenchido)
        [MaxLength(7)]
        public string? Placa { get; set; }
        [MaxLength(17)]
        public string? Chassi { get; set; }
        public string? QRCode { get; set; }
        
        // Histórico
        public virtual ICollection<HistoricoMoto> Historicos { get; set; } = [];
        
        [NotMapped]
        public HistoricoMoto? VinculoAtual => Historicos.FirstOrDefault(h => h.Ativo);

        public Moto(MotoType type, string? placa = null, string? chassi = null, string? qrCode = null)
        {
            Type = type;
            Placa = placa;
            Chassi = chassi;
            QRCode = qrCode;
        }

        public Moto() { }
    }
}