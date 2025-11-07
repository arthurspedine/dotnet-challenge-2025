using System.ComponentModel.DataAnnotations;

namespace Motoflow.Web.Models.DTOs
{
    public class MotoDTO
    {
        public long Id { get; set; }
        public MotoType Type { get; set; }
        public string? Placa { get; set; }
        public string? Chassi { get; set; }
        public string? QRCode { get; set; }

        public static MotoDTO FromMoto(Moto moto)
        {
            return new MotoDTO
            {
                Id = moto.Id,
                Type = moto.Type,
                Placa = moto.Placa,
                Chassi = moto.Chassi,
                QRCode = moto.QRCode,
            };
        }
    }

    public class CreateMotoDTO
    {
        [Required]
        public MotoType Type { get; set; }
        
        public string? Placa { get; set; }
        public string? Chassi { get; set; }
        public string? QRCode { get; set; }
        
        public MotoType GetMotoType() => Type;
    }
}
