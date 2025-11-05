using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Motoflow.Data;
using Motoflow.Models;
using Motoflow.Models.DTOs;
using BCrypt.Net;

namespace Motoflow.Services
{
    /// <summary>
    /// Serviço de autenticação e gerenciamento de usuários
    /// </summary>
    public class AuthService
    {
        private readonly OracleDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(OracleDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Registra um novo usuário no sistema
        /// </summary>
        public async Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO registerDto)
        {
            // Verifica se o email já está em uso
            var existingUserByEmail = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == registerDto.Email.ToLower());

            if (existingUserByEmail != null)
            {
                throw new InvalidOperationException("Email já está em uso.");
            }

            // Verifica se o username já está em uso
            var existingUserByUsername = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == registerDto.Username.ToLower());

            if (existingUserByUsername != null)
            {
                throw new InvalidOperationException("Nome de usuário já está em uso.");
            }

            // Cria o hash da senha usando BCrypt
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password, workFactor: 12);

            // Cria o novo usuário
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email.ToLower(),
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Novo usuário registrado: {Username} ({Email})", user.Username, user.Email);

            // Gera o token JWT
            var token = GenerateJwtToken(user);
            var expiresAt = DateTime.UtcNow.AddMinutes(GetTokenExpirationMinutes());

            return new AuthResponseDTO
            {
                Token = token,
                TokenType = "Bearer",
                ExpiresAt = expiresAt,
                User = UserInfoDTO.FromUser(user)
            };
        }

        /// <summary>
        /// Autentica um usuário existente
        /// </summary>
        public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO loginDto)
        {
            // Busca o usuário pelo email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == loginDto.Email.ToLower());

            if (user == null)
            {
                _logger.LogWarning("Tentativa de login com email inexistente: {Email}", loginDto.Email);
                throw new UnauthorizedAccessException("Email ou senha inválidos.");
            }

            // Verifica a senha usando BCrypt
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                _logger.LogWarning("Tentativa de login com senha incorreta para: {Email}", loginDto.Email);
                throw new UnauthorizedAccessException("Email ou senha inválidos.");
            }

            _logger.LogInformation("Login bem-sucedido: {Username} ({Email})", user.Username, user.Email);

            // Gera o token JWT
            var token = GenerateJwtToken(user);
            var expiresAt = DateTime.UtcNow.AddMinutes(GetTokenExpirationMinutes());

            return new AuthResponseDTO
            {
                Token = token,
                TokenType = "Bearer",
                ExpiresAt = expiresAt,
                User = UserInfoDTO.FromUser(user)
            };
        }

        /// <summary>
        /// Gera um token JWT para o usuário
        /// </summary>
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] 
                ?? throw new InvalidOperationException("JWT SecretKey não configurada.");
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(GetTokenExpirationMinutes()),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Obtém o tempo de expiração do token em minutos
        /// </summary>
        private int GetTokenExpirationMinutes()
        {
            var expirationMinutes = _configuration.GetSection("JwtSettings")["ExpirationMinutes"];
            return int.TryParse(expirationMinutes, out var minutes) ? minutes : 60;
        }

        /// <summary>
        /// Busca um usuário pelo ID
        /// </summary>
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        /// <summary>
        /// Verifica se um usuário existe
        /// </summary>
        public async Task<bool> IsUserActiveAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user != null;
        }
    }
}
