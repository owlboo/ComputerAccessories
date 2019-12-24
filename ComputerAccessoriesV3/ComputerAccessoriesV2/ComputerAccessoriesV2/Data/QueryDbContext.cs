using ComputerAccessoriesV2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComputerAccessoriesV2.Models;
using ComputerAccessoriesV2.ViewModels.DbQueryModels;
using Microsoft.EntityFrameworkCore;

namespace ComputerAccessoriesV2.Data
{
    public partial class QueryDbContext : DbContext
    {
        public QueryDbContext()
        {
        }

        public QueryDbContext(DbContextOptions<QueryDbContext> options)
            : base(options)
        {
        }


        public virtual DbSet<CategoryShoppingModel> CategoryShoppingModel
        {
            get;
            set;
        }
        public virtual DbSet<UserInformationModel> UserInformationModels { get; set; }

        public virtual DbSet<CampaignProduct> CampaignProducts { get; set; }

        public virtual DbSet<ProductAvailableForCompaign> ProductAvailableForCompaign { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=35.194.1.21,1433;Initial Catalog=ComputerAccessoriesV2;Persist Security Info=False;User ID=sqllol;Password=}eVcQQPHRPoRLIf4;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<CategoryShoppingModel>(entity => { entity.HasNoKey(); });
            modelBuilder.Entity<UserInformationModel>(entity => { entity.HasNoKey(); });
            modelBuilder.Entity<CampaignProduct>().HasKey(pc => new { pc.ProductId });
            modelBuilder.Entity<ProductAvailableForCompaign>(entity => { entity.HasNoKey(); });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
