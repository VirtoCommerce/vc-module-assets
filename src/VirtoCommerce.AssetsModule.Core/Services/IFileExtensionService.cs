using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.AssetsModule.Core.Services;

public interface IFileExtensionService
{
    Task<IList<string>> GetWhiteListAsync();
    Task<IList<string>> GetBlackListAsync();
    Task<bool> IsExtensionAllowedAsync(string path);
}
