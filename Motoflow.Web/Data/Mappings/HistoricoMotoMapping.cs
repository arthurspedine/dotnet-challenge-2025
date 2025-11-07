using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motoflow.Web.Models;

namespace Motoflow.Web.Data.Mappings
{
    public class HistoricoMotoMapping : IEntityTypeConfiguration<HistoricoMoto>
    {
        public void Configure(EntityTypeBuilder<HistoricoMoto> builder)
        {
            builder.ToTable("MOTOFLOW_HISTORICOS_MOTOS");

            builder.HasKey(m => m.Id);

            builder.HasOne(v => v.Moto)
                   .WithMany(m => m.Historicos)
                   .HasForeignKey(v => v.MotoId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            builder.HasOne(v => v.Area)
                   .WithMany(a => a.Historicos)
                   .HasForeignKey(v => v.AreaId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();
            
            // Índice único para garantir que uma moto não tenha mais de um histórico ativo
            builder.HasIndex(v => new { v.MotoId, v.DataSaida })
                   .HasFilter("\"DataSaida\" IS NULL")
                   .IsUnique();

            builder.Property(v => v.ObservacaoEntrada)
                   .IsRequired()
                   .HasMaxLength(500);
            
            builder.Property(v => v.ObservacaoSaida)
                   .IsRequired(false)
                   .HasMaxLength(500);

            builder.Property(v => v.DataEntrada)
                   .IsRequired()
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}