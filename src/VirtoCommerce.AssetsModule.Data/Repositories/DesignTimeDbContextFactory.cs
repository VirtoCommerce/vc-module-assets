using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VirtoCommerce.AssetsModule.Data.Repositories
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AssetsDbContext>
    {
        public AssetsDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AssetsDbContext>();

            builder.UseSqlServer("Data Source=(local);Initial Catalog=VirtoCommerce3;Persist Security Info=True;User ID=virto;Password=virto;MultipleActiveResultSets=True;Connect Timeout=30");

            return new AssetsDbContext(builder.Options);
        }
    }
}
