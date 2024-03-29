using System;
using Microsoft.Extensions.Options;
using VirtoCommerce.AssetsModule.Core.Services;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.AssetsModule.Core.Assets
{
    [Obsolete("Use IFileExtensionService", DiagnosticId = "VC0008", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
    public abstract class BasicBlobProvider
    {
        private readonly FileExtensionService _fileExtensionService;

        protected BasicBlobProvider(IOptions<PlatformOptions> platformOptions, ISettingsManager settingsManager)
        {
            _fileExtensionService = new FileExtensionService(platformOptions, settingsManager);
        }

        public virtual bool IsExtensionBlacklisted(string path)
        {
            var allowed = _fileExtensionService.IsExtensionAllowedAsync(path).GetAwaiter().GetResult();
            return !allowed;
        }
    }
}
