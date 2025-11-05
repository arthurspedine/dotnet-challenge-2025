using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motoflow.Models;

namespace Motoflow.Data.Mappings
{
    /// <summary>
    /// Configuração de mapeamento para a entidade User
    /// </summary>
    public class UserMapping : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("TB_USERS");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .HasColumnName("USER_ID")
                .ValueGeneratedOnAdd();

            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("USERNAME")
                .HasColumnType("VARCHAR2(50)");

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("EMAIL")
                .HasColumnType("VARCHAR2(100)");

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("PASSWORD_HASH")
                .HasColumnType("VARCHAR2(255)");

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasColumnName("CREATED_AT")
                .HasDefaultValueSql("SYSTIMESTAMP");

            // Índices únicos para username e email
            builder.HasIndex(u => u.Username)
                .IsUnique()
                .HasDatabaseName("UK_USERS_USERNAME");

            builder.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("UK_USERS_EMAIL");
        }
    }
}
