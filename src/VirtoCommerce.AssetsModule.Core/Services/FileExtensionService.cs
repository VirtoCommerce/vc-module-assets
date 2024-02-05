using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.AssetsModule.Core.Services;

public class FileExtensionService : IFileExtensionService
{
    private readonly StringComparer _ignoreCase = StringComparer.OrdinalIgnoreCase;
    private readonly string _whiteListSettingName = PlatformConstants.Settings.Security.FileExtensionsWhiteList.Name;
    private readonly string _blackListSettingName = PlatformConstants.Settings.Security.FileExtensionsBlackList.Name;

    private readonly ISettingsManager _settingsManager;
    private readonly PlatformOptions _platformOptions;

    public FileExtensionService(IOptions<PlatformOptions> platformOptions, ISettingsManager settingsManager)
    {
        _platformOptions = platformOptions.Value;
        _settingsManager = settingsManager;
    }

    public virtual async Task<bool> IsExtensionAllowedAsync(string extension)
    {
        var whiteList = await GetWhiteListAsync();
        if (whiteList.Count != 0)
        {
            return whiteList.Contains(extension, _ignoreCase);
        }

        var blackList = await GetBlackListAsync();
        return blackList.Count == 0 || !blackList.Contains(extension, _ignoreCase);
    }


    public Task<IList<string>> GetWhiteListAsync()
    {
        return CombineSettingValuesAsync(_whiteListSettingName, _platformOptions.FileExtensionsWhiteList);
    }

    public Task<IList<string>> GetBlackListAsync()
    {
        return CombineSettingValuesAsync(_blackListSettingName, _platformOptions.FileExtensionsBlackList);
    }

    protected async Task<IList<string>> CombineSettingValuesAsync(string settingName, IList<string> otherValues)
    {
        var setting = await _settingsManager.GetObjectSettingAsync(settingName);

        var blackList = setting.AllowedValues.OfType<string>()
            .Union(otherValues, _ignoreCase)
            .OrderBy(x => x)
            .ToList()
            .AsReadOnly();

        return blackList;
    }
}
