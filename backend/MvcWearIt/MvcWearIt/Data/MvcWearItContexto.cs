using Microsoft.EntityFrameworkCore;
using MvcWearIt.Models;

namespace MvcWearIt.Data
{
    public class MvcWearItContexto : DbContext
    {
        public MvcWearItContexto(DbContextOptions<MvcWearItContexto> options) : base(options)
        {
        }

        public DbSet<Juego> Juegos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Detalle> Detalles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tipos de datos decimales para evitar warnings
            modelBuilder.Entity<Producto>().Property(p => p.Precio).HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<Detalle>().Property(d => d.Precio).HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<Detalle>().Property(d => d.Descuento).HasColumnType("decimal(18, 2)");

            // Configuración manual para evitar bucles de borrado (Cascade paths) en SQL Server
            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Juego)
                .WithMany(j => j.Productos)
                .HasForeignKey(p => p.JuegoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

