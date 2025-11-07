namespace Motoflow.Web.Models.DTOs
{
    /// <summary>
    /// DTO para requisição de previsão de ocupação
    /// </summary>
    public class OccupancyPredictionRequestDTO
    {
        /// <summary>
        /// ID da área
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// Dia da semana para previsão (0-6, onde 0 = Domingo)
        /// </summary>
        public int DiaSemana { get; set; }
    }

    /// <summary>
    /// DTO para resposta de previsão de ocupação
    /// </summary>
    public class OccupancyPredictionResponseDTO
    {
        /// <summary>
        /// ID da área
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// Nome da área
        /// </summary>
        public string NomeArea { get; set; } = string.Empty;

        /// <summary>
        /// Capacidade total da área
        /// </summary>
        public int Capacidade { get; set; }

        /// <summary>
        /// Número atual de motos
        /// </summary>
        public int MotosAtuais { get; set; }

        /// <summary>
        /// Taxa de ocupação prevista (%)
        /// </summary>
        public float TaxaOcupacaoPrevista { get; set; }

        /// <summary>
        /// Status da previsão
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Recomendação baseada na previsão
        /// </summary>
        public string Recomendacao { get; set; } = string.Empty;
    }
}
