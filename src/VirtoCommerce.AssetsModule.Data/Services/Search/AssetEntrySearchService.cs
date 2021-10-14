using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.AssetsModule.Data.Model;
using VirtoCommerce.AssetsModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace VirtoCommerce.AssetsModule.Data.Services
{
    public class AssetEntrySearchService : SearchService<AssetEntrySearchCriteria, AssetEntrySearchResult, AssetEntry, AssetEntryEntity>
    {
        public AssetEntrySearchService(Func<IAssetsRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache,
            ICrudService<AssetEntry> assetEntryService) : base(repositoryFactory, platformMemoryCache, assetEntryService)
        {

        }

        protected override IQueryable<AssetEntryEntity> BuildQuery(IRepository repository, AssetEntrySearchCriteria criteria)
        {
            var query = ((IAssetsRepository)repository).AssetEntries;

            if (!string.IsNullOrEmpty(criteria.SearchPhrase))
            {
                query = query.Where(x =>
                    x.Name.Contains(criteria.SearchPhrase) || x.RelativeUrl.Contains(criteria.SearchPhrase));
            }

            if (!string.IsNullOrEmpty(criteria.LanguageCode))
            {
                query = query.Where(x => x.LanguageCode == criteria.LanguageCode);
            }

            if (!string.IsNullOrEmpty(criteria.Group))
            {
                query = query.Where(x => x.Group == criteria.Group);
            }

            if (!criteria.Tenants.IsNullOrEmpty())
            {
                var tenants = criteria.Tenants.Where(x => x.IsValid).ToArray();
                if (tenants.Any())
                {
                    var tenantsStrings = tenants.Select(x => x.ToString());
                    query = query.Where(x => tenantsStrings.Contains(x.TenantId + "_" + x.TenantType));
                }
            }

            return query;
        }
    }
}
