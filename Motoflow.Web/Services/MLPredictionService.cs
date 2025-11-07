using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Motoflow.Web.Data;
using Motoflow.Web.Models.DTOs;
using Motoflow.Web.Models.ML;

namespace Motoflow.Web.Services
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
        private const string ModelPath = "motoflow-ml-model.zip";

        public MLPredictionService(IServiceScopeFactory scopeFactory, ILogger<MLPredictionService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _mlContext = new MLContext(seed: 0);
            LoadModel();
        }

        /// <summary>
        /// Carrega o modelo de ML pré-treinado do arquivo .zip
        /// </summary>
        private void LoadModel()
        {
            try
            {
                if (!File.Exists(ModelPath))
                {
                    _logger.LogError($"Arquivo de modelo não encontrado: {ModelPath}");
                    _logger.LogWarning("Execute o projeto Motoflow.Trainer para gerar o modelo");
                    throw new FileNotFoundException($"Modelo ML não encontrado em {ModelPath}. Execute 'dotnet run --project Motoflow.Trainer' para gerar o modelo.");
                }

                // Carrega o modelo do arquivo .zip
                _model = _mlContext.Model.Load(ModelPath, out var modelInputSchema);
                
                _logger.LogInformation($"Modelo de ML carregado com sucesso de {ModelPath}");
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar modelo de ML");
                throw new InvalidOperationException("Falha ao carregar o modelo de ML", ex);
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
