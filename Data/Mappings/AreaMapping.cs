using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motoflow.Models;

namespace Motoflow.Data.Mappings
{
    public class AreaMapping : IEntityTypeConfiguration<Area>
    {
        public void Configure(EntityTypeBuilder<Area> builder)
        {
           builder.ToTable("MOTOFLOW_AREAS");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Identificador)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(a => a.CapacidadeMaxima)
                .IsRequired();

            builder.HasOne(a => a.Patio)
                .WithMany(p => p.Areas)
                .HasForeignKey(a => a.PatioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ignora a propriedade Motos no mapeamento, já que não é um relacionamento direto
            builder.Ignore(a => a.Motos);
        }
    }
}