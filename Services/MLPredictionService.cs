using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Motoflow.Data;
using Motoflow.Models.DTOs;
using Motoflow.Models.ML;

namespace Motoflow.Services
{
    /// <summary>
    /// Serviço de Machine Learning para previsões relacionadas ao Motoflow
    /// </summary>
    public class MLPredictionService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MLPredictionService> _logger;
        private readonly MLContext _mlContext;
        private ITransformer? _model;

        public MLPredictionService(IServiceScopeFactory scopeFactory, ILogger<MLPredictionService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _mlContext = new MLContext(seed: 0);
            TrainModel();
        }

        /// <summary>
        /// Treina o modelo de ML com dados sintéticos
        /// </summary>
        private void TrainModel()
        {
            try
            {
                // Dados de treinamento sintéticos
                var trainingData = new List<AreaOccupancyData>
                {
                    // Segunda-feira - alta ocupação
                    new AreaOccupancyData { Capacidade = 50, MotosAtuais = 45, MediaEntradasDia = 20, MediaSaidasDia = 18, DiaSemana = 1, TaxaOcupacao = 90 },
                    new AreaOccupancyData { Capacidade = 100, MotosAtuais = 85, MediaEntradasDia = 35, MediaSaidasDia = 30, DiaSemana = 1, TaxaOcupacao = 85 },
                    new AreaOccupancyData { Capacidade = 75, MotosAtuais = 60, MediaEntradasDia = 25, MediaSaidasDia = 22, DiaSemana = 1, TaxaOcupacao = 80 },
                    
                    // Terça-feira - alta ocupação
                    new AreaOccupancyData { Capacidade = 50, MotosAtuais = 43, MediaEntradasDia = 19, MediaSaidasDia = 17, DiaSemana = 2, TaxaOcupacao = 86 },
                    new AreaOccupancyData { Capacidade = 100, MotosAtuais = 82, MediaEntradasDia = 33, MediaSaidasDia = 28, DiaSemana = 2, TaxaOcupacao = 82 },
                    
                    // Quarta-feira - média ocupação
                    new AreaOccupancyData { Capacidade = 50, MotosAtuais = 35, MediaEntradasDia = 15, MediaSaidasDia = 14, DiaSemana = 3, TaxaOcupacao = 70 },
                    new AreaOccupancyData { Capacidade = 100, MotosAtuais = 65, MediaEntradasDia = 28, MediaSaidasDia = 25, DiaSemana = 3, TaxaOcupacao = 65 },
                    new AreaOccupancyData { Capacidade = 75, MotosAtuais = 48, MediaEntradasDia = 20, MediaSaidasDia = 18, DiaSemana = 3, TaxaOcupacao = 64 },
                    
                    // Quinta-feira - média ocupação
                    new AreaOccupancyData { Capacidade = 50, MotosAtuais = 33, MediaEntradasDia = 14, MediaSaidasDia = 13, DiaSemana = 4, TaxaOcupacao = 66 },
                    new AreaOccupancyData { Capacidade = 100, MotosAtuais = 62, MediaEntradasDia = 26, MediaSaidasDia = 24, DiaSemana = 4, TaxaOcupacao = 62 },
                    
                    // Sexta-feira - baixa ocupação
                    new AreaOccupancyData { Capacidade = 50, MotosAtuais = 20, MediaEntradasDia = 10, MediaSaidasDia = 12, DiaSemana = 5, TaxaOcupacao = 40 },
                    new AreaOccupancyData { Capacidade = 100, MotosAtuais = 45, MediaEntradasDia = 18, MediaSaidasDia = 22, DiaSemana = 5, TaxaOcupacao = 45 },
                    new AreaOccupancyData { Capacidade = 75, MotosAtuais = 30, MediaEntradasDia = 12, MediaSaidasDia = 15, DiaSemana = 5, TaxaOcupacao = 40 },
                    
                    // Sábado - muito baixa ocupação
                    new AreaOccupancyData { Capacidade = 50, MotosAtuais = 10, MediaEntradasDia = 5, MediaSaidasDia = 8, DiaSemana = 6, TaxaOcupacao = 20 },
                    new AreaOccupancyData { Capacidade = 100, MotosAtuais = 25, MediaEntradasDia = 10, MediaSaidasDia = 15, DiaSemana = 6, TaxaOcupacao = 25 },
                    
                    // Domingo - muito baixa ocupação
                    new AreaOccupancyData { Capacidade = 50, MotosAtuais = 8, MediaEntradasDia = 4, MediaSaidasDia = 6, DiaSemana = 0, TaxaOcupacao = 16 },
                    new AreaOccupancyData { Capacidade = 100, MotosAtuais = 20, MediaEntradasDia = 8, MediaSaidasDia = 12, DiaSemana = 0, TaxaOcupacao = 20 },
                };

                var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

                // Pipeline de treinamento usando regressão (FastTree)
                var pipeline = _mlContext.Transforms.Concatenate("Features",
                        nameof(AreaOccupancyData.Capacidade),
                        nameof(AreaOccupancyData.MotosAtuais),
                        nameof(AreaOccupancyData.MediaEntradasDia),
                        nameof(AreaOccupancyData.MediaSaidasDia),
                        nameof(AreaOccupancyData.DiaSemana))
                    .Append(_mlContext.Regression.Trainers.FastTree(
                        labelColumnName: nameof(AreaOccupancyData.TaxaOcupacao),
                        numberOfLeaves: 20,
                        numberOfTrees: 100));

                // Treina o modelo
                _model = pipeline.Fit(dataView);

                _logger.LogInformation("Modelo de ML treinado com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao treinar modelo de ML");
            }
        }

        /// <summary>
        /// Prevê a taxa de ocupação de uma área para um dia específico
        /// </summary>
        public async Task<OccupancyPredictionResponseDTO> PredictOccupancyAsync(int areaId, int diaSemana)
        {
            if (_model == null)
            {
                throw new InvalidOperationException("Modelo de ML não foi treinado");
            }

            // Cria um scope para acessar o DbContext
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<OracleDbContext>();

            // Busca informações da área
            var area = await context.Areas
                .Include(a => a.Patio)
                .FirstOrDefaultAsync(a => a.Id == areaId);

            if (area == null)
            {
                throw new KeyNotFoundException($"Área com ID {areaId} não encontrada");
            }

            // Calcula estatísticas do histórico
            var historicos = await context.HistoricoMotos
                .Where(h => h.AreaId == areaId)
                .ToListAsync();

            var motosAtuais = historicos.Count(h => h.DataSaida == null);
            
            // Calcula médias (se não houver dados, usa valores padrão)
            var mediaEntradasDia = historicos.Any() 
                ? historicos.Count(h => h.DataEntrada.Date == DateTime.Today) 
                : 10f;
            
            var mediaSaidasDia = historicos.Any() 
                ? historicos.Count(h => h.DataSaida?.Date == DateTime.Today) 
                : 8f;

            // Prepara dados para previsão
            var input = new AreaOccupancyData
            {
                Capacidade = area.CapacidadeMaxima,
                MotosAtuais = motosAtuais,
                MediaEntradasDia = mediaEntradasDia,
                MediaSaidasDia = mediaSaidasDia,
                DiaSemana = diaSemana,
                TaxaOcupacao = 0 // Será previsto
            };

            // Cria o prediction engine
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<AreaOccupancyData, AreaOccupancyPrediction>(_model);
            
            // Faz a previsão
            var prediction = predictionEngine.Predict(input);

            // Garante que a taxa está entre 0 e 100
            var taxaPrevista = Math.Max(0, Math.Min(100, prediction.TaxaOcupacaoPrevista));

            // Determina status e recomendação
            var (status, recomendacao) = GetStatusAndRecommendation(taxaPrevista);

            return new OccupancyPredictionResponseDTO
            {
                AreaId = (int)area.Id,
                NomeArea = area.Identificador,
                Capacidade = area.CapacidadeMaxima,
                MotosAtuais = motosAtuais,
                TaxaOcupacaoPrevista = taxaPrevista,
                Status = status,
                Recomendacao = recomendacao
            };
        }

        /// <summary>
        /// Determina o status e a recomendação baseado na taxa de ocupação prevista
        /// </summary>
        private (string Status, string Recomendacao) GetStatusAndRecommendation(float taxaOcupacao)
        {
            return taxaOcupacao switch
            {
                >= 90 => ("Crítico", "Área quase lotada. Considere direcionar motos para outras áreas."),
                >= 75 => ("Alto", "Alta ocupação esperada. Monitore de perto a disponibilidade."),
                >= 50 => ("Moderado", "Ocupação moderada. Boa disponibilidade de espaço."),
                >= 25 => ("Baixo", "Baixa ocupação. Muita disponibilidade de espaço."),
                _ => ("Muito Baixo", "Ocupação mínima. Área amplamente disponível.")
            };
        }
    }
}
