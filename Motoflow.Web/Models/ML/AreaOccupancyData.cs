using Microsoft.ML.Data;

namespace Motoflow.Web.Models.ML
{
    /// <summary>
    /// Dados de entrada para previsão de ocupação de área
    /// </summary>
    public class AreaOccupancyData
    {
        /// <summary>
        /// Capacidade total da área
        /// </summary>
        [LoadColumn(0)]
        public float Capacidade { get; set; }

        /// <summary>
        /// Número atual de motos na área
        /// </summary>
        [LoadColumn(1)]
        public float MotosAtuais { get; set; }

        /// <summary>
        /// Média de entradas por dia
        /// </summary>
        [LoadColumn(2)]
        public float MediaEntradasDia { get; set; }

        /// <summary>
        /// Média de saídas por dia
        /// </summary>
        [LoadColumn(3)]
        public float MediaSaidasDia { get; set; }

        /// <summary>
        /// Dia da semana (0-6, onde 0 = Domingo)
        /// </summary>
        [LoadColumn(4)]
        public float DiaSemana { get; set; }

        /// <summary>
        /// Taxa de ocupação real (0-100)
        /// </summary>
        [LoadColumn(5)]
        public float TaxaOcupacao { get; set; }
    }

    /// <summary>
    /// Resultado da previsão de ocupação
    /// </summary>
    public class AreaOccupancyPrediction
    {
        /// <summary>
        /// Taxa de ocupação prevista (0-100)P
        /// </summary>
        [ColumnName("Score")]
        public float TaxaOcupacaoPrevista { get; set; }
    }
}
