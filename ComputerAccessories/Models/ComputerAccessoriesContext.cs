using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ComputerAccessories.Models
{
    public partial class ComputerAccessoriesContext : DbContext
    {
        public ComputerAccessoriesContext()
        {
        }

        public ComputerAccessoriesContext(DbContextOptions<ComputerAccessoriesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblAttribute> TblAttribute { get; set; }
        public virtual DbSet<TblBrand> TblBrand { get; set; }
        public virtual DbSet<TblCategory> TblCategory { get; set; }
        public virtual DbSet<TblProduct> TblProduct { get; set; }
        public virtual DbSet<TblProductAttributes> TblProductAttributes { get; set; }
        public virtual DbSet<TblRoles> TblRoles { get; set; }
        public virtual DbSet<TblUserRole> TblUserRole { get; set; }
        public virtual DbSet<TblUsers> TblUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=104.211.43.223,1433;Initial Catalog=ComputerAccessories;Persist Security Info=False;User ID=sa;Password=SiO4B3@M$*bBSO&6;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TblAttribute>(entity =>
            {
                entity.ToTable("tbl_Attribute");

                entity.Property(e => e.AttributeName).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.TblAttribute)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_tbl_Attribute_tbl_Category");
            });

            modelBuilder.Entity<TblBrand>(entity =>
            {
                entity.ToTable("tbl_Brand");

                entity.Property(e => e.BrandName).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Logo).HasMaxLength(100);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<TblCategory>(entity =>
            {
                entity.ToTable("tbl_Category");

                entity.Property(e => e.CategoryName).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<TblProduct>(entity =>
            {
                entity.ToTable("tbl_Product");

                entity.Property(e => e.Color).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FullDescription).HasMaxLength(500);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Origin).HasMaxLength(50);

                entity.Property(e => e.Price)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ProductName).HasMaxLength(100);

                entity.Property(e => e.Quantity).HasDefaultValueSql("((0))");

                entity.Property(e => e.ShortDescription).HasMaxLength(200);

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.TblProduct)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_tbl_Product_tbl_Brand");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.TblProduct)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_tbl_Product_tbl_Category");
            });

            modelBuilder.Entity<TblProductAttributes>(entity =>
            {
                entity.ToTable("tbl_Product_Attributes");

                entity.Property(e => e.Value).HasMaxLength(100);

                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.TblProductAttributes)
                    .HasForeignKey(d => d.AttributeId)
                    .HasConstraintName("FK_tbl_Product_Attributes_tbl_Attribute");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.TblProductAttributes)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_tbl_Product_Attributes_tbl_Product");
            });

            modelBuilder.Entity<TblRoles>(entity =>
            {
                entity.ToTable("tbl_Roles");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<TblUserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.ToTable("tbl_User_Role");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.TblUserRole)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_AspNetUserRoles_AspNetRoles_RoleId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblUserRole)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_AspNetUserRoles_AspNetUsers_UserId");
            });

            modelBuilder.Entity<TblUsers>(entity =>
            {
                entity.ToTable("tbl_Users");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DisplayName).HasMaxLength(100);

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.Password).HasMaxLength(30);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
