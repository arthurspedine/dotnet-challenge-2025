using System.ComponentModel.DataAnnotations;

namespace Motoflow.Models.DTOs
{
    /// <summary>
    /// DTO para registro de novos usuários
    /// </summary>
    public class RegisterRequestDTO
    {
        /// <summary>
        /// Nome de usuário (único)
        /// </summary>
        [Required(ErrorMessage = "Nome de usuário é obrigatório")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Nome de usuário deve ter entre 3 e 50 caracteres")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email do usuário (único)
        /// </summary>
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Senha do usuário
        /// </summary>
        [Required(ErrorMessage = "Senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Senha deve ter entre 6 e 100 caracteres")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Confirmação da senha
        /// </summary>
        [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
        [Compare("Password", ErrorMessage = "As senhas não coincidem")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para login de usuários
    /// </summary>
    public class LoginRequestDTO
    {
        /// <summary>
        /// Email do usuário
        /// </summary>
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Senha do usuário
        /// </summary>
        [Required(ErrorMessage = "Senha é obrigatória")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO de resposta de autenticação
    /// </summary>
    public class AuthResponseDTO
    {
        /// <summary>
        /// Token JWT gerado
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Tipo do token (Bearer)
        /// </summary>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>
        /// Data de expiração do token
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Informações do usuário
        /// </summary>
        public UserInfoDTO User { get; set; } = null!;
    }

    /// <summary>
    /// DTO com informações básicas do usuário
    /// </summary>
    public class UserInfoDTO
    {
        /// <summary>
        /// ID do usuário
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome de usuário
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email do usuário
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Converte um User para UserInfoDTO
        /// </summary>
        public static UserInfoDTO FromUser(User user)
        {
            return new UserInfoDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };
        }
    }
}
