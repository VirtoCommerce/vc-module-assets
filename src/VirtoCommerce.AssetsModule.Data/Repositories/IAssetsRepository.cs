using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.AssetsModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.AssetsModule.Data.Repositories
{
    public interface IAssetsRepository : IRepository
    {
        IQueryable<AssetEntryEntity> AssetEntries { get; }
        Task<IList<AssetEntryEntity>> GetAssetsByIdsAsync(IList<string> ids);
    }
}
