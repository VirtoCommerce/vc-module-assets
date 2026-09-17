using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.AssetsModule.Web.Controllers;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Exceptions;
using Xunit;

namespace VirtoCommerce.AssetsModule.Tests.Controllers
{
    /// <summary>
    /// VCST-6016 (Defect 1): the local-storage upload endpoint must not allow the
    /// multipart file name to write outside the configured upload folder.
    /// </summary>
    [Trait("Category", "Unit")]
    public class AssetsControllerSecurityTests : IDisposable
    {
        private readonly string _uploadFolder;

        public AssetsControllerSecurityTests()
        {
            _uploadFolder = Path.Combine(Path.GetTempPath(), "AssetsControllerSecurityTests", Guid.NewGuid().ToString("N"), "uploads");
            Directory.CreateDirectory(_uploadFolder);
        }

        public void Dispose()
        {
            var testRoot = Directory.GetParent(_uploadFolder)?.FullName;
            if (testRoot != null && Directory.Exists(testRoot))
            {
                Directory.Delete(testRoot, recursive: true);
            }
        }

        [Fact]
        public async Task UploadAssetToLocalFileSystem_WithTraversalFileName_DoesNotWriteOutsideUploadFolder()
        {
            // Arrange
            const string boundary = "----vcst6016boundary";
            var fileName = "../escaped-" + Guid.NewGuid().ToString("N") + ".txt";
            var controller = BuildController($"multipart/form-data; boundary={boundary}", BuildMultipartBody(boundary, fileName, "poc-content"));

            // The path the traversal name would resolve to, one level above the upload folder.
            var escapedPath = Path.GetFullPath(Path.Combine(_uploadFolder, fileName));

            // Act - a hardened endpoint rejects the name with a PlatformException.
            try
            {
                await controller.UploadAssetToLocalFileSystemAsync();
            }
            catch (PlatformException)
            {
                // expected once the fix is in place
            }

            // Assert - the security invariant: nothing was written outside the upload folder.
            Assert.False(File.Exists(escapedPath), $"Arbitrary file write outside upload folder: {escapedPath}");
        }

        [Fact]
        public async Task UploadAssetToLocalFileSystem_WithRootedFileName_DoesNotWriteToAbsolutePath()
        {
            // Arrange - an absolute file name; Path.Combine would otherwise discard the upload root.
            var absoluteTarget = Path.Combine(Path.GetTempPath(), "AssetsControllerSecurityTests", "rooted-" + Guid.NewGuid().ToString("N") + ".txt");
            const string boundary = "----vcst6016boundary";
            var controller = BuildController($"multipart/form-data; boundary={boundary}", BuildMultipartBody(boundary, absoluteTarget, "poc-content"));

            // Act
            try
            {
                await controller.UploadAssetToLocalFileSystemAsync();
            }
            catch (PlatformException)
            {
                // expected once the fix is in place
            }

            // Assert
            Assert.False(File.Exists(absoluteTarget), $"Arbitrary file write to absolute path: {absoluteTarget}");
        }

        [Fact]
        public async Task UploadAssetToLocalFileSystem_WithNormalFileName_WritesInsideUploadFolder()
        {
            // Arrange - a legitimate upload must keep working.
            const string boundary = "----vcst6016boundary";
            var fileName = "normal-" + Guid.NewGuid().ToString("N") + ".txt";
            var controller = BuildController($"multipart/form-data; boundary={boundary}", BuildMultipartBody(boundary, fileName, "hello"));

            // Act
            var result = await controller.UploadAssetToLocalFileSystemAsync();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            Assert.True(File.Exists(Path.Combine(_uploadFolder, fileName)), "Legitimate upload must be written inside the upload folder.");
        }

        private AssetsController BuildController(string contentType, Stream body)
        {
            var platformOptions = Options.Create(new PlatformOptions { LocalUploadFolderPath = _uploadFolder });

            var controller = new AssetsController(
                Mock.Of<IBlobStorageProvider>(),
                Mock.Of<IBlobUrlResolver>(),
                platformOptions,
                Mock.Of<IHttpClientFactory>());

            var httpContext = new DefaultHttpContext();
            httpContext.Request.ContentType = contentType;
            httpContext.Request.Body = body;

            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            return controller;
        }

        private static Stream BuildMultipartBody(string boundary, string fileName, string content)
        {
            var builder = new StringBuilder();
            builder.Append("--").Append(boundary).Append("\r\n");
            builder.Append("Content-Disposition: form-data; name=\"file\"; filename=\"").Append(fileName).Append("\"\r\n");
            builder.Append("Content-Type: text/plain\r\n");
            builder.Append("\r\n");
            builder.Append(content).Append("\r\n");
            builder.Append("--").Append(boundary).Append("--\r\n");

            return new MemoryStream(Encoding.UTF8.GetBytes(builder.ToString()));
        }
    }
}
