using Microsoft.EntityFrameworkCore;
using Motoflow.Web.Models;

namespace Motoflow.Web.Data 
{
    public class OracleDbContext : DbContext
    {
        public OracleDbContext(DbContextOptions<OracleDbContext> options) : base(options) { }

        public DbSet<Moto> Motos { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Patio> Patios { get; set; }
        public DbSet<HistoricoMoto> HistoricoMotos { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OracleDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
