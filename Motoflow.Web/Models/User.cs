using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Motoflow.Web.Models
{
    /// <summary>
    /// Representa um usuário do sistema
    /// </summary>
    public class User
    {
        /// <summary>
        /// Identificador único do usuário
        /// </summary>
        [Key]
        [Column("USER_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Nome de usuário (único)
        /// </summary>
        [Required(ErrorMessage = "Nome de usuário é obrigatório")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Nome de usuário deve ter entre 3 e 50 caracteres")]
        [Column("USERNAME", TypeName = "VARCHAR2(50)")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email do usuário (único)
        /// </summary>
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        [Column("EMAIL", TypeName = "VARCHAR2(100)")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Hash da senha do usuário (usando BCrypt)
        /// </summary>
        [Required]
        [Column("PASSWORD_HASH", TypeName = "VARCHAR2(255)")]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Data de criação do usuário
        /// </summary>
        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
