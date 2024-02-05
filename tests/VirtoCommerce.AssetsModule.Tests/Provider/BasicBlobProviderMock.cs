using Microsoft.Extensions.Options;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.AssetsModule.Tests.Provider
{
#pragma warning disable VC0008 // Type or member is obsolete
    public class BasicBlobProviderMock : BasicBlobProvider
#pragma warning restore VC0008 // Type or member is obsolete
    {
        public BasicBlobProviderMock(IOptions<PlatformOptions> platformOptions, ISettingsManager settingsManager)
            : base(platformOptions, settingsManager)
        {
        }
    }
}
