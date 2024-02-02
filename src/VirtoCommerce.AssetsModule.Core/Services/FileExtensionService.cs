using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Core.Settings.Events;

namespace VirtoCommerce.AssetsModule.Core.Services;

public class FileExtensionService : IFileExtensionService
{
    private readonly StringComparer _ignoreCase = StringComparer.OrdinalIgnoreCase;
    private readonly string _whiteListSettingName = PlatformConstants.Settings.Security.FileExtensionsWhiteList.Name;
    private readonly string _blackListSettingName = PlatformConstants.Settings.Security.FileExtensionsBlackList.Name;
    private static IList<string> _whiteList;
    private static IList<string> _blackList;

    private readonly ISettingsManager _settingsManager;
    private readonly PlatformOptions _platformOptions;

    public FileExtensionService(IOptions<PlatformOptions> platformOptions, ISettingsManager settingsManager)
    {
        _platformOptions = platformOptions.Value;
        _settingsManager = settingsManager;

        _whiteList ??= GetWhiteList().GetAwaiter().GetResult();
        _blackList ??= GetBlackList().GetAwaiter().GetResult();
    }

    public IList<string> WhiteList => _whiteList;
    public IList<string> BlackList => _blackList;

    public virtual bool IsExtensionAllowed(string extension)
    {
        if (WhiteList.Count != 0)
        {
            return WhiteList.Contains(extension, _ignoreCase);
        }

        return BlackList.Count == 0 || !BlackList.Contains(extension, _ignoreCase);
    }

    public virtual async Task Handle(ObjectSettingChangedEvent message)
    {
        var whiteListChanged = false;
        var blackListChanged = false;

        foreach (var changedEntry in message.ChangedEntries)
        {
            if (changedEntry.NewEntry.Name == _whiteListSettingName)
            {
                whiteListChanged = true;
            }
            else if (changedEntry.NewEntry.Name == _blackListSettingName)
            {
                blackListChanged = true;
            }
        }

        if (whiteListChanged)
        {
            _whiteList = await GetWhiteList();
        }

        if (blackListChanged)
        {
            _blackList = await GetBlackList();
        }
    }


    protected Task<IList<string>> GetWhiteList()
    {
        return CombineSettingValues(_whiteListSettingName, _platformOptions.FileExtensionsWhiteList);
    }

    protected Task<IList<string>> GetBlackList()
    {
        return CombineSettingValues(_blackListSettingName, _platformOptions.FileExtensionsBlackList);
    }

    protected async Task<IList<string>> CombineSettingValues(string settingName, IList<string> otherValues)
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
