using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using VirtoCommerce.AssetsModule.Data.Model;

namespace VirtoCommerce.AssetsModule.Data.Repositories
{
    public class AssetsDbContext : DbContextWithTriggers
    {
        public AssetsDbContext(DbContextOptions<AssetsDbContext> options)
            : base(options)
        {
        }

        protected AssetsDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AssetEntryEntity>().ToTable("AssetEntry").HasKey(x => x.Id);
            modelBuilder.Entity<AssetEntryEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            modelBuilder.Entity<AssetEntryEntity>().Property(x => x.CreatedBy).HasMaxLength(64);
            modelBuilder.Entity<AssetEntryEntity>().Property(x => x.ModifiedBy).HasMaxLength(64);
            modelBuilder.Entity<AssetEntryEntity>().HasIndex(x => new { x.RelativeUrl, x.Name })
                .IsUnique(false)
                .HasDatabaseName("IX_AssetEntry_RelativeUrl_Name");

            base.OnModelCreating(modelBuilder);


            // Allows configuration for an entity type for different database types.
            // Applies configuration from all <see cref="IEntityTypeConfiguration{TEntity}" in VirtoCommerce.AssetsModule.Data.XXX project. /> 
            switch (this.Database.ProviderName)
            {
                case "Pomelo.EntityFrameworkCore.MySql":
                    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.AssetsModule.Data.MySql"));
                    break;
                case "Npgsql.EntityFrameworkCore.PostgreSQL":
                    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.AssetsModule.Data.PostgreSql"));
                    break;
                case "Microsoft.EntityFrameworkCore.SqlServer":
                    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.AssetsModule.Data.SqlServer"));
                    break;
            }
        }

    }


}
