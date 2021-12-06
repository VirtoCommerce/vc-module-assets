using FluentAssertions;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core;
using Xunit;
using Moq;
using VirtoCommerce.Platform.Core.Settings;
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
        public void IsExtensionBlacklisted_Blacklisted_ReturnTrue()
        {
            // Arrange
            SetupAllowedValues(Security.FileExtensionsBlackList, ".pdf");
            SetupAllowedValues(Security.FileExtensionsWhiteList);
            var service = new BasicBlobProviderMock(_platformOptionsMock.Object, _settingsManagerMock.Object);

            // Act
            var result = service.IsExtensionBlacklisted(".pdf");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsExtensionBlacklisted_NotWhitelisted_ReturnTrue()
        {
            // Arrange
            SetupAllowedValues(Security.FileExtensionsBlackList);
            SetupAllowedValues(Security.FileExtensionsWhiteList, ".txt");
            var service = new BasicBlobProviderMock(_platformOptionsMock.Object, _settingsManagerMock.Object);

            // Act
            var result = service.IsExtensionBlacklisted(".pdf");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsExtensionBlacklisted_Whitelisted_ReturnFalse()
        {
            // Arrange
            SetupAllowedValues(Security.FileExtensionsBlackList);
            SetupAllowedValues(Security.FileExtensionsWhiteList, ".txt");
            var service = new BasicBlobProviderMock(_platformOptionsMock.Object, _settingsManagerMock.Object);

            // Act
            var result = service.IsExtensionBlacklisted(".txt");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsExtensionBlacklisted_ListsIsEmpty_ReturnFalse()
        {
            // Arrange
            SetupAllowedValues(Security.FileExtensionsBlackList);
            SetupAllowedValues(Security.FileExtensionsWhiteList);
            var service = new BasicBlobProviderMock(_platformOptionsMock.Object, _settingsManagerMock.Object);

            // Act
            var result = service.IsExtensionBlacklisted(".png");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsExtensionBlacklisted_BlacklistedAndWhitelisted_ReturnTrue()
        {
            // Arrange
            SetupAllowedValues(Security.FileExtensionsBlackList, ".mp3");
            SetupAllowedValues(Security.FileExtensionsWhiteList, ".mp3");
            var service = new BasicBlobProviderMock(_platformOptionsMock.Object, _settingsManagerMock.Object);

            // Act
            var result = service.IsExtensionBlacklisted(".mp3");

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
