using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.AssetsModule.Core.Services;

public interface IAssetEntrySearchService : ISearchService<AssetEntrySearchCriteria, AssetEntrySearchResult, AssetEntry>
{
}
