using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.AssetsModule.Core.Events;
using VirtoCommerce.AssetsModule.Core.Services;
using VirtoCommerce.AssetsModule.Data.Model;
using VirtoCommerce.AssetsModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace VirtoCommerce.AssetsModule.Data.Services
{
    public class AssetEntryService : CrudService<AssetEntry, AssetEntryEntity, AssetEntryChangingEvent, AssetEntryChangedEvent>, IAssetEntryService
    {
        private readonly IBlobUrlResolver _blobUrlResolver;

        public AssetEntryService(
            Func<IAssetsRepository> repositoryFactory,
            IPlatformMemoryCache platformMemoryCache,
            IEventPublisher eventPublisher,
            IBlobUrlResolver blobUrlResolver)
            : base(repositoryFactory, platformMemoryCache, eventPublisher)
        {
            _blobUrlResolver = blobUrlResolver;
        }

        protected override Task<IList<AssetEntryEntity>> LoadEntities(IRepository repository, IList<string> ids, string responseGroup)
        {
            return ((IAssetsRepository)repository).GetAssetsByIdsAsync(ids);
        }

        protected override AssetEntry ProcessModel(string responseGroup, AssetEntryEntity entity, AssetEntry model)
        {
            model.BlobInfo.Url = _blobUrlResolver.GetAbsoluteUrl(model.BlobInfo.RelativeUrl);
            return model;
        }
    }
}
