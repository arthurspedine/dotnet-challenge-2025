using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motoflow.Models;

namespace Motoflow.Data.Mappings
{
    public class PatioMapping : IEntityTypeConfiguration<Patio>
    {
        public void Configure(EntityTypeBuilder<Patio> builder)
        {
            builder.ToTable("MOTOFLOW_PATIOS");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nome)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(p => p.Localizacao)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasMany(p => p.Areas)
                .WithOne(a => a.Patio)
                .HasForeignKey(a => a.PatioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}