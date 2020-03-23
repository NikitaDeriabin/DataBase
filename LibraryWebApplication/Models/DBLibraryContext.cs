using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LibraryWebApplication
{
    public partial class DBLibraryContext : DbContext
    {
        public DBLibraryContext()
        {
        }

        public DBLibraryContext(DbContextOptions<DBLibraryContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Colors> Colors { get; set; }
        public virtual DbSet<Companies> Companies { get; set; }
        public virtual DbSet<CompanyProducts> CompanyProducts { get; set; }
        public virtual DbSet<Countries> Countries { get; set; }
        public virtual DbSet<Filials> Filials { get; set; }
        public virtual DbSet<GenManagers> GenManagers { get; set; }
        public virtual DbSet<ModelsOfProduct> ModelsOfProduct { get; set; }
        public virtual DbSet<Products> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=DESKTOP-76GMDMG; Database=DBLibrary; Trusted_Connection=True; ");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Colors>(entity =>
            {
                entity.ToTable("COLORS");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Companies>(entity =>
            {
                entity.ToTable("COMPANIES");

                entity.HasIndex(e => e.GenManagerId)
                    .HasName("IX_COMPANIES")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CountryId).HasColumnName("Country_ID");

                entity.Property(e => e.GenManagerId).HasColumnName("Gen_Manager_ID");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Year).HasColumnType("date");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Companies)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK_COMPANIES_COUNTRIES");

                entity.HasOne(d => d.GenManager)
                    .WithOne(p => p.Companies)
                    .HasForeignKey<Companies>(d => d.GenManagerId)
                    .HasConstraintName("FK_COMPANIES_GEN_MANAGERS");
            });

            modelBuilder.Entity<CompanyProducts>(entity =>
            {
                entity.ToTable("COMPANY_PRODUCTS");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CompanyId).HasColumnName("COMPANY_ID");

                entity.Property(e => e.ProductId).HasColumnName("PRODUCT_ID");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CompanyProducts)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_COMPANY_PRODUCTS_COMPANIES");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.CompanyProducts)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_COMPANY_PRODUCTS_PRODUCTS");
            });

            modelBuilder.Entity<Countries>(entity =>
            {
                entity.ToTable("COUNTRIES");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Filials>(entity =>
            {
                entity.ToTable("FILIALS");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CompanyId).HasColumnName("Company_ID");

                entity.Property(e => e.CountryId).HasColumnName("Country_ID");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Filials)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_FILIALS_COMPANIES");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Filials)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK_FILIALS_COUNTRIES");
            });

            modelBuilder.Entity<GenManagers>(entity =>
            {
                entity.ToTable("GEN_MANAGERS");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CountryId).HasColumnName("Country_ID");

                entity.Property(e => e.Information).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.YearBirth)
                    .HasColumnName("Year_Birth")
                    .HasColumnType("date");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.GenManagers)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK_GEN_MANAGERS_COUNTRIES");
            });

            modelBuilder.Entity<ModelsOfProduct>(entity =>
            {
                entity.ToTable("MODELS_OF_PRODUCT");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ColorId).HasColumnName("Color_ID");

                entity.Property(e => e.CompProdId).HasColumnName("Comp_Prod_ID");

                entity.Property(e => e.Information).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.Color)
                    .WithMany(p => p.ModelsOfProduct)
                    .HasForeignKey(d => d.ColorId)
                    .HasConstraintName("FK_MODELS_OF_PRODUCT_COLORS");

                entity.HasOne(d => d.CompProd)
                    .WithMany(p => p.ModelsOfProduct)
                    .HasForeignKey(d => d.CompProdId)
                    .HasConstraintName("FK_MODELS_OF_PRODUCT_COMPANY_PRODUCTS");
            });

            modelBuilder.Entity<Products>(entity =>
            {
                entity.ToTable("PRODUCTS");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Information).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
