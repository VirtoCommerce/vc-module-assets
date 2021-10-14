using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.AssetsModule.Data.Repositories;
using VirtoCommerce.AssetsModule.Data.Services;
using VirtoCommerce.AssetsModule.Web.Swagger;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Data.Extensions;

namespace VirtoCommerce.AssetsModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSwaggerGen(c =>
            {
                c.OperationFilter<FileUploadOperationFilter>();
            });

            serviceCollection.AddDbContext<AssetsDbContext>((provider, options) =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                options.UseSqlServer(configuration.GetConnectionString(ModuleInfo.Id) ?? configuration.GetConnectionString("VirtoCommerce"));
            });
            serviceCollection.AddTransient<IAssetsRepository, AssetsRepository>();
            serviceCollection.AddSingleton<Func<IAssetsRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<IAssetsRepository>());
            serviceCollection.AddTransient<ICrudService<AssetEntry>, AssetEntryService>();
            serviceCollection.AddTransient<ISearchService<AssetEntrySearchCriteria, AssetEntrySearchResult, AssetEntry>, AssetEntrySearchService>();

        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<AssetsDbContext>())
                {
                    dbContext.Database.MigrateIfNotApplied("20000000000000_UpdateAssetsV3");
                    dbContext.Database.EnsureCreated();
                    dbContext.Database.Migrate();
                }
            }

        }

        public void Uninstall()
        {
            //Nothing special here
        }


    }
}
