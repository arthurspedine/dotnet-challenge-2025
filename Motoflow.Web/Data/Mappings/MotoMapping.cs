using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motoflow.Web.Models;

namespace Motoflow.Web.Data.Mappings
{
    public class MotoMapping : IEntityTypeConfiguration<Moto>
    {
        public void Configure(EntityTypeBuilder<Moto> builder)
        {
            builder.ToTable("MOTOFLOW_MOTOS");

            builder.HasKey(m => m.Id);

            var maxEnumLength = Enum.GetNames(typeof(MotoType)).Max(n => n.Length);
            builder.Property(m => m.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(maxEnumLength);

            builder.Property(m => m.Placa)
                .HasMaxLength(7)
                .IsRequired(false);

            builder.Property(m => m.Chassi)
                .HasMaxLength(17)
                .IsRequired(false);

            builder.Property(m => m.QRCode)
                .HasMaxLength(255)
                .IsRequired(false);

            builder.HasIndex(m => m.Placa)
                .IsUnique()
                .HasFilter("\"Placa\" IS NOT NULL");

            builder.HasIndex(m => m.Chassi)
                .IsUnique()
                .HasFilter("\"Chassi\" IS NOT NULL");

            builder.HasIndex(m => m.QRCode)
                .IsUnique()
                .HasFilter("\"QRCode\" IS NOT NULL");

            // Configuração de relacionamento com histórico
            builder.HasMany(m => m.Historicos)
                .WithOne(h => h.Moto)
                .HasForeignKey(h => h.MotoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}