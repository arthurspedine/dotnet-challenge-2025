using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Motoflow.Models.DTOs;
using Motoflow.Services;
using System.Net;

namespace Motoflow.Controllers
{
    /// <summary>
    /// Controller para previsões usando Machine Learning
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class MLPredictionController : ControllerBase
    {
        private readonly MLPredictionService _mlService;
        private readonly ILogger<MLPredictionController> _logger;

        public MLPredictionController(MLPredictionService mlService, ILogger<MLPredictionController> logger)
        {
            _mlService = mlService;
            _logger = logger;
        }

        /// <summary>
        /// Prevê a taxa de ocupação de uma área para um dia específico usando ML.NET
        /// </summary>
        /// <param name="request">Dados para previsão (AreaId e DiaSemana)</param>
        /// <returns>Previsão de ocupação com recomendações</returns>
        /// <response code="200">Previsão realizada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="404">Área não encontrada</response>
        /// <response code="500">Erro interno do servidor</response>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     POST /api/mlprediction/occupancy
        ///     {
        ///        "areaId": 1,
        ///        "diaSemana": 1
        ///     }
        ///
        /// DiaSemana: 0 = Domingo, 1 = Segunda, 2 = Terça, 3 = Quarta, 4 = Quinta, 5 = Sexta, 6 = Sábado
        /// 
        /// O modelo usa ML.NET (FastTree Regression) para prever a taxa de ocupação baseado em:
        /// - Capacidade da área
        /// - Número atual de motos
        /// - Média histórica de entradas/saídas
        /// - Dia da semana
        /// </remarks>
        [HttpPost("occupancy")]
        [ProducesResponseType(typeof(OccupancyPredictionResponseDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<OccupancyPredictionResponseDTO>> PredictOccupancy(
            [FromBody] OccupancyPredictionRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (request.DiaSemana < 0 || request.DiaSemana > 6)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Status = (int)HttpStatusCode.BadRequest,
                        Title = "Dia da semana inválido",
                        Detail = "O dia da semana deve estar entre 0 (Domingo) e 6 (Sábado)."
                    });
                }

                var prediction = await _mlService.PredictOccupancyAsync(request.AreaId, request.DiaSemana);

                _logger.LogInformation(
                    "Previsão de ocupação realizada: Área {AreaId}, Taxa Prevista: {Taxa}%", 
                    request.AreaId, 
                    prediction.TaxaOcupacaoPrevista);

                return Ok(prediction);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Área não encontrada: {AreaId}", request.AreaId);
                return NotFound(new ProblemDetails
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Title = "Área não encontrada",
                    Detail = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Erro ao realizar previsão de ML");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "Erro no modelo de ML",
                    Detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao realizar previsão");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "Erro interno do servidor",
                    Detail = "Ocorreu um erro ao processar sua solicitação."
                });
            }
        }

        /// <summary>
        /// Obtém informações sobre o modelo de ML utilizado
        /// </summary>
        /// <returns>Informações do modelo</returns>
        /// <response code="200">Informações retornadas com sucesso</response>
        /// <response code="401">Não autorizado</response>
        [HttpGet("model-info")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        public ActionResult GetModelInfo()
        {
            var info = new
            {
                ModelType = "FastTree Regression",
                Framework = "ML.NET 3.0.1",
                Purpose = "Previsão de taxa de ocupação de áreas",
                Features = new[]
                {
                    "Capacidade da área",
                    "Número atual de motos",
                    "Média de entradas por dia",
                    "Média de saídas por dia",
                    "Dia da semana (0-6)"
                },
                Output = "Taxa de ocupação prevista (0-100%)",
                TrainingData = "Dados sintéticos baseados em padrões típicos de ocupação",
                Accuracy = "Modelo treinado para demonstração - Para produção, use dados reais"
            };

            return Ok(info);
        }
    }
}
