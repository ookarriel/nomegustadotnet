using System;
using System.Collections.Generic;
using SistemaManejoBar.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace SistemaManejoBar.Models;

public partial class BarraDbContext : IdentityDbContext<IdentityUser>
{
    

    public BarraDbContext(DbContextOptions<BarraDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CategoriaCoctel> CategoriaCoctels { get; set; }

    public virtual DbSet<Coctel> Coctels { get; set; }

    public virtual DbSet<Cristalerium> Cristaleria { get; set; }

    public virtual DbSet<DetalleRecetum> DetalleReceta { get; set; }

    public virtual DbSet<Ingrediente> Ingredientes { get; set; }

    public virtual DbSet<TipoIngrediente> TipoIngredientes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
                base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CategoriaCoctel>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PK__categori__8A3D240C8DCE5215");

            entity.ToTable("categoriaCoctel");

            entity.HasIndex(e => e.NombreCategoria, "UQ__categori__788BF0FA1773A1F2").IsUnique();

            entity.Property(e => e.IdCategoria).HasColumnName("idCategoria");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.NombreCategoria)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombreCategoria");
        });

        modelBuilder.Entity<Coctel>(entity =>
        {
            entity.HasKey(e => e.IdCoctel).HasName("PK__coctel__490EE0B8AC7D8D3D");

            entity.ToTable("coctel");

            entity.HasIndex(e => e.NombreCoctel, "UQ__coctel__119308C3376FCC7C").IsUnique();

            entity.Property(e => e.IdCoctel).HasColumnName("idCoctel");
            entity.Property(e => e.Foto)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("foto");
            entity.Property(e => e.IdCategoria).HasColumnName("idCategoria");
            entity.Property(e => e.IdCristaleria).HasColumnName("idCristaleria");
            entity.Property(e => e.Instrucciones)
                .IsUnicode(false)
                .HasColumnName("instrucciones");
            entity.Property(e => e.NombreCoctel)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombreCoctel");
            entity.Property(e => e.PrecioVenta).HasColumnName("precioVenta");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Coctels)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__coctel__idCatego__5812160E");

            entity.HasOne(d => d.IdCristaleriaNavigation).WithMany(p => p.Coctels)
                .HasForeignKey(d => d.IdCristaleria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__coctel__idCrista__59063A47");
        });

        modelBuilder.Entity<Cristalerium>(entity =>
        {
            entity.HasKey(e => e.IdCristaleria).HasName("PK__cristale__80DCDE0C2E3D9921");

            entity.ToTable("cristaleria");

            entity.HasIndex(e => e.NombreCristaleria, "UQ__cristale__4CC6C0C8C376C714").IsUnique();

            entity.Property(e => e.IdCristaleria).HasColumnName("idCristaleria");
            entity.Property(e => e.CapacidadOz).HasColumnName("capacidadOz");
            entity.Property(e => e.NombreCristaleria)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombreCristaleria");
        });

        modelBuilder.Entity<DetalleRecetum>(entity =>
        {
            entity.HasKey(e => e.IdDetalleReceta).HasName("PK__detalleR__7A7D8379DF2B628A");

            entity.ToTable("detalleReceta");

            entity.Property(e => e.IdDetalleReceta).HasColumnName("idDetalleReceta");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.IdCoctel).HasColumnName("idCoctel");
            entity.Property(e => e.IdIngrediente).HasColumnName("idIngrediente");

            entity.HasOne(d => d.IdCoctelNavigation).WithMany(p => p.DetalleReceta)
                .HasForeignKey(d => d.IdCoctel)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__detalleRe__idCoc__5BE2A6F2");

            entity.HasOne(d => d.IdIngredienteNavigation).WithMany(p => p.DetalleReceta)
                .HasForeignKey(d => d.IdIngrediente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__detalleRe__idIng__5CD6CB2B");
        });

        modelBuilder.Entity<Ingrediente>(entity =>
        {
            entity.HasKey(e => e.IdIngrediente).HasName("PK__ingredie__563C0D33A647086D");

            entity.ToTable("ingrediente");

            entity.HasIndex(e => e.NombreIngrediente, "UQ__ingredie__92BF05284D116A0F").IsUnique();

            entity.Property(e => e.IdIngrediente).HasColumnName("idIngrediente");
            entity.Property(e => e.IdTipoIngrediente).HasColumnName("idTipoIngrediente");
            entity.Property(e => e.NombreIngrediente)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombreIngrediente");
            entity.Property(e => e.Stock).HasColumnName("stock");
            entity.Property(e => e.Unidad)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("unidad");

            entity.HasOne(d => d.IdTipoIngredienteNavigation).WithMany(p => p.Ingredientes)
                .HasForeignKey(d => d.IdTipoIngrediente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ingredien__idTip__5441852A");
        });

        modelBuilder.Entity<TipoIngrediente>(entity =>
        {
            entity.HasKey(e => e.IdTipoIngrediente).HasName("PK__tipoIngr__17766B0815E208DF");

            entity.ToTable("tipoIngrediente");

            entity.HasIndex(e => e.NombreTipoIngrediente, "UQ__tipoIngr__6D5D16D4B73CCDA2").IsUnique();

            entity.Property(e => e.IdTipoIngrediente).HasColumnName("idTipoIngrediente");
            entity.Property(e => e.NombreTipoIngrediente)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombreTipoIngrediente");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
