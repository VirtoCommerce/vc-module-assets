using Microsoft.Extensions.Options;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.AssetsModule.Tests.Provider
{
    public class BasicBlobProviderMock : BasicBlobProvider
    {
        public BasicBlobProviderMock(IOptions<PlatformOptions> platformOptions, ISettingsManager settingsManager)
            : base(platformOptions, settingsManager)
        {
        }
    }
}
