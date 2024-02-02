using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.AssetsModule.Core.Services;
using VirtoCommerce.AssetsModule.Data.MySql;
using VirtoCommerce.AssetsModule.Data.PostgreSql;
using VirtoCommerce.AssetsModule.Data.Repositories;
using VirtoCommerce.AssetsModule.Data.Services;
using VirtoCommerce.AssetsModule.Data.SqlServer;
using VirtoCommerce.AssetsModule.Web.Swagger;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings.Events;
using VirtoCommerce.Platform.Data.Extensions;

namespace VirtoCommerce.AssetsModule.Web
{
    public class Module : IModule, IHasConfiguration
    {
        public ManifestModuleInfo ModuleInfo { get; set; }
        public IConfiguration Configuration { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSwaggerGen(c =>
            {
                c.OperationFilter<FileUploadOperationFilter>();
            });

            serviceCollection.AddDbContext<AssetsDbContext>((provider, options) =>
            {
                var databaseProvider = Configuration.GetValue("DatabaseProvider", "SqlServer");
                var connectionString = Configuration.GetConnectionString(ModuleInfo.Id) ?? Configuration.GetConnectionString("VirtoCommerce");

                switch (databaseProvider)
                {
                    case "MySql":
                        options.UseMySqlDatabase(connectionString);
                        break;
                    case "PostgreSql":
                        options.UsePostgreSqlDatabase(connectionString);
                        break;
                    default:
                        options.UseSqlServerDatabase(connectionString);
                        break;
                }
            });

            serviceCollection.AddTransient<IAssetsRepository, AssetsRepository>();
            serviceCollection.AddSingleton<Func<IAssetsRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<IAssetsRepository>());
            serviceCollection.AddTransient<IAssetEntryService, AssetEntryService>();
            serviceCollection.AddTransient<IAssetEntrySearchService, AssetEntrySearchService>();
            serviceCollection.AddSingleton<IFileExtensionService, FileExtensionService>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            var serviceProvider = appBuilder.ApplicationServices;

            var handlerRegistrar = serviceProvider.GetService<IHandlerRegistrar>();
            handlerRegistrar.RegisterHandler<ObjectSettingChangedEvent>(async (message, _) => await serviceProvider.GetService<IFileExtensionService>().Handle(message));

            var databaseProvider = Configuration.GetValue("DatabaseProvider", "SqlServer");

            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<AssetsDbContext>())
                {
                    if (databaseProvider == "SqlServer")
                    {
                        dbContext.Database.MigrateIfNotApplied("20000000000000_UpdateAssetsV3");
                    }
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
