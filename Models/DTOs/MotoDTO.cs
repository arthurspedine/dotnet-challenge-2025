using System.ComponentModel.DataAnnotations;

namespace Motoflow.Models.DTOs
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

        private MotoType _type;

        [Required]
        public string Type
        {
            get => _type.ToString();
            set
            {
                if (Enum.TryParse<MotoType>(value, true, out MotoType result))
                {
                    _type = result;
                }
                else
                {
                    throw new ArgumentException($"Tipo de moto inválido. Valores aceitos: {string.Join(", ", Enum.GetNames(typeof(MotoType)))}");
                }
            }
        }
        public string? Placa { get; set; }
        public string? Chassi { get; set; }
        public string? QRCode { get; set; }
    }
}
