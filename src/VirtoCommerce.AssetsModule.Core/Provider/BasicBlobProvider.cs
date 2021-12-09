using System;
using System.Linq;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.AssetsModule.Core.Assets
{
    public abstract class BasicBlobProvider
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IOptions<PlatformOptions> _platformOptions;
        protected BasicBlobProvider(IOptions<PlatformOptions> platformOptions, ISettingsManager settingsManager)
        {
            _platformOptions = platformOptions;
            _settingsManager = settingsManager;
        }

        // Blacklisted or whitelisted extension
        public virtual bool IsExtensionBlacklisted(string path)
        {
            var blackList = _platformOptions.Value.FileExtensionsBlackList.Union(
                _settingsManager?.GetObjectSettingAsync(PlatformConstants.Settings.Security.FileExtensionsBlackList.Name).Result.AllowedValues.Cast<string>() ?? new string[0]);
            var blacklisted = (blackList.Any(x => path.Trim().EndsWith(x.Trim(), StringComparison.OrdinalIgnoreCase)));
            var blackListCount = blackList.Count();

            var whiteList = _platformOptions.Value.FileExtensionsWhiteList.Union(
                _settingsManager?.GetObjectSettingAsync(PlatformConstants.Settings.Security.FileExtensionsWhiteList.Name).Result.AllowedValues.Cast<string>() ?? new string[0]);
            var whitelisted = (whiteList.Any(x => path.Trim().EndsWith(x.Trim(), StringComparison.OrdinalIgnoreCase)));
            var whiteListCount = whiteList.Count();

            if (whiteListCount != 0)
            {
                if (blackListCount != 0 && blacklisted) return true;
                return !whitelisted;
            }
            else
            {
                if (blackListCount != 0) return blacklisted;
            }
            return false;
        }
    }
}
