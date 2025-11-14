using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using VirtoCommerce.AssetsModule.Core.Services;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Settings;
using Xunit;
using static VirtoCommerce.Platform.Core.PlatformConstants.Settings;

namespace VirtoCommerce.AssetsModule.Tests.Provider
{
    public class BasicBlobProviderTests
    {
        private readonly Mock<IOptions<PlatformOptions>> _platformOptionsMock;
        private readonly Mock<ISettingsManager> _settingsManagerMock;
        private readonly PlatformOptions _platformOptions;

        public BasicBlobProviderTests()
        {
            _platformOptionsMock = new Mock<IOptions<PlatformOptions>>();
            _settingsManagerMock = new Mock<ISettingsManager>();
            _platformOptions = new PlatformOptions { FileExtensionsBlackList = new string[0] };
            _platformOptions = new PlatformOptions { FileExtensionsWhiteList = new string[0] };
            _platformOptionsMock.SetupGet(x => x.Value).Returns(_platformOptions);
        }

        [Fact]
        public async Task IsExtensionBlacklisted_Blacklisted_ReturnTrue()
        {
            // Arrange
            SetupAllowedValues(Security.FileExtensionsBlackList, ".pdf");
            SetupAllowedValues(Security.FileExtensionsWhiteList);
            var service = new FileExtensionService(_platformOptionsMock.Object, _settingsManagerMock.Object);

            // Act
            var result = await service.IsExtensionAllowedAsync(".pdf");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsExtensionBlacklisted_NotWhitelisted_ReturnTrue()
        {
            // Arrange
            SetupAllowedValues(Security.FileExtensionsBlackList);
            SetupAllowedValues(Security.FileExtensionsWhiteList, ".txt");
            var service = new FileExtensionService(_platformOptionsMock.Object, _settingsManagerMock.Object);

            // Act
            var result = await service.IsExtensionAllowedAsync(".pdf");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsExtensionBlacklisted_Whitelisted_ReturnFalse()
        {
            // Arrange
            SetupAllowedValues(Security.FileExtensionsBlackList);
            SetupAllowedValues(Security.FileExtensionsWhiteList, ".txt");
            var service = new FileExtensionService(_platformOptionsMock.Object, _settingsManagerMock.Object);

            // Act
            var result = await service.IsExtensionAllowedAsync(".txt");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsExtensionBlacklisted_ListsIsEmpty_ReturnFalse()
        {
            // Arrange
            SetupAllowedValues(Security.FileExtensionsBlackList);
            SetupAllowedValues(Security.FileExtensionsWhiteList);
            var service = new FileExtensionService(_platformOptionsMock.Object, _settingsManagerMock.Object);

            // Act
            var result = await service.IsExtensionAllowedAsync(".png");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsExtensionBlacklisted_BlacklistedAndWhitelisted_ReturnFalse()
        {
            // Arrange
            SetupAllowedValues(Security.FileExtensionsBlackList, ".mp3");
            SetupAllowedValues(Security.FileExtensionsWhiteList, ".mp3");
            var service = new FileExtensionService(_platformOptionsMock.Object, _settingsManagerMock.Object);

            // Act
            var result = await service.IsExtensionAllowedAsync(".mp3");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsExtensionBlacklisted_NotBlacklistedAndWhitelisted_ReturnTrue()
        {
            // Arrange
            SetupAllowedValues(Security.FileExtensionsBlackList, ".mp3");
            SetupAllowedValues(Security.FileExtensionsWhiteList, ".txt");
            var service = new FileExtensionService(_platformOptionsMock.Object, _settingsManagerMock.Object);

            // Act
            var result = await service.IsExtensionAllowedAsync(".pdf");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsExtensionBlacklisted_NotBlacklistedAndWhiteListEmpty_ReturnFalse()
        {
            // Arrange
            SetupAllowedValues(Security.FileExtensionsBlackList, ".mp3");
            SetupAllowedValues(Security.FileExtensionsWhiteList);
            var service = new FileExtensionService(_platformOptionsMock.Object, _settingsManagerMock.Object);

            // Act
            var result = await service.IsExtensionAllowedAsync(".pdf");

            // Assert
            result.Should().BeTrue();
        }

        private void SetupAllowedValues(SettingDescriptor extensionList, params string[] values)
        {
            _settingsManagerMock.Setup(x => x.GetObjectSettingAsync(extensionList.Name, null, null)).ReturnsAsync(new ObjectSettingEntry()
            {
                AllowedValues = values
            });
        }
    }
}
