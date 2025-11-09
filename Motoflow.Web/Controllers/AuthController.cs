using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Motoflow.Web.Models.DTOs;
using Motoflow.Web.Services;
using System.Net;

namespace Motoflow.Web.Controllers
{
    /// <summary>
    /// Controller para autenticação e gerenciamento de usuários
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Registra um novo usuário no sistema
        /// </summary>
        /// <param name="registerDto">Dados do novo usuário</param>
        /// <returns>Token JWT e informações do usuário criado</returns>
        /// <response code="201">Usuário criado com sucesso</response>
        /// <response code="400">Dados inválidos ou usuário já existe</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<AuthResponseDTO>> Register([FromBody] RegisterRequestDTO registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _authService.RegisterAsync(registerDto);

                return CreatedAtAction(
                    nameof(Register), 
                    new { id = response.User.Id }, 
                    response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Erro ao registrar usuário: {Message}", ex.Message);
                return BadRequest(new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Erro no registro",
                    Detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao registrar usuário");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "Erro interno do servidor",
                    Detail = "Ocorreu um erro ao processar sua solicitação."
                });
            }
        }

        /// <summary>
        /// Autentica um usuário existente
        /// </summary>
        /// <param name="loginDto">Credenciais de login</param>
        /// <returns>Token JWT e informações do usuário</returns>
        /// <response code="200">Login realizado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="401">Credenciais inválidas</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginRequestDTO loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _authService.LoginAsync(loginDto);

                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Tentativa de login não autorizada: {Message}", ex.Message);
                return Unauthorized(new ProblemDetails
                {
                    Status = (int)HttpStatusCode.Unauthorized,
                    Title = "Não autorizado",
                    Detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao realizar login");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "Erro interno do servidor",
                    Detail = "Ocorreu um erro ao processar sua solicitação."
                });
            }
        }

        /// <summary>
        /// Verifica se o token JWT é válido (requer autenticação)
        /// </summary>
        /// <returns>Informações do usuário autenticado</returns>
        /// <response code="200">Token válido</response>
        /// <response code="401">Token inválido ou expirado</response>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserInfoDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<UserInfoDTO>> GetCurrentUser()
        {
            try
            {
                // Obtém o ID do usuário a partir das claims do token
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new ProblemDetails
                    {
                        Status = (int)HttpStatusCode.Unauthorized,
                        Title = "Não autorizado",
                        Detail = "Token inválido."
                    });
                }

                var user = await _authService.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Title = "Usuário não encontrado",
                        Detail = "O usuário associado ao token não foi encontrado."
                    });
                }

                return Ok(UserInfoDTO.FromUser(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter informações do usuário atual");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "Erro interno do servidor",
                    Detail = "Ocorreu um erro ao processar sua solicitação."
                });
            }
        }
    }
}
