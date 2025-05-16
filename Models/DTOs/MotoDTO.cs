namespace Motoflow.Models.DTOs
{
    public class MotoDTO
    {
        public long Id { get; set; }
        public MotoType Type { get; set; }

        public string? Placa { get; set; }
        public string? Chassi { get; set; }
        public string? QRCode { get; set; }

        public long? HistoricoAtivoId { get; set; }
    }
}
