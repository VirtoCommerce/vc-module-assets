using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.AssetsModule.Core.Swagger;
using VirtoCommerce.AssetsModule.Web.Helpers;
using VirtoCommerce.AssetsModule.Web.Validators;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Exceptions;
using VirtoCommerce.Platform.Data.Helpers;

using UrlHelpers = VirtoCommerce.Platform.Core.Extensions.UrlHelperExtensions;

namespace VirtoCommerce.AssetsModule.Web.Controllers
{
    [Route("api/assets")]
    public class AssetsController : Controller
    {
        private readonly IBlobStorageProvider _blobProvider;
        private readonly IBlobUrlResolver _urlResolver;
        private readonly PlatformOptions _platformOptions;
        private readonly IHttpClientFactory _httpClientFactory;

        private static readonly FormOptions _defaultFormOptions = new();

        public AssetsController(IBlobStorageProvider blobProvider, IBlobUrlResolver urlResolver, IOptions<PlatformOptions> platformOptions, IHttpClientFactory httpClientFactory)
        {
            _blobProvider = blobProvider;
            _urlResolver = urlResolver;
            _platformOptions = platformOptions.Value;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// This method used to upload files on local disk storage in special uploads folder
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("localstorage")]
        [DisableFormValueModelBinding]
        [DisableRequestSizeLimit]
        [Authorize(PlatformConstants.Security.Permissions.AssetCreate)]
        public async Task<ActionResult<BlobInfo[]>> UploadAssetToLocalFileSystemAsync()
        {
            // Now supports downloading one file, find a solution for downloading multiple files
            // https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-8.0
            var result = new List<BlobInfo>();

            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }
            var uploadPath = Path.GetFullPath(_platformOptions.LocalUploadFolderPath);
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);

            var section = await reader.ReadNextSectionAsync();
            if (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);

                if (hasContentDispositionHeader && MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                {
                    var fileName = contentDisposition.FileName.Value;
                    var targetFilePath = Path.Combine(uploadPath, fileName);

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    await using (var targetStream = System.IO.File.Create(targetFilePath))
                    {
                        await section.Body.CopyToAsync(targetStream);
                    }

                    var blobInfo = AbstractTypeFactory<BlobInfo>.TryCreateInstance();
                    blobInfo.Name = fileName;
                    //Use only file name as Url, for further access to these files need use PlatformOptions.LocalUploadFolderPath
                    blobInfo.Url = fileName;
                    blobInfo.ContentType = MimeTypeResolver.ResolveContentType(fileName);
                    result.Add(blobInfo);
                }
            }

            return Ok(result.ToArray());
        }

        /// <summary>
        /// Upload assets to the folder
        /// </summary>
        /// <remarks>
        /// Request body can contain multiple files.
        /// </remarks>
        /// <param name="folderUrl">Parent folder url (relative or absolute).</param>
        /// <param name="url">Url for uploaded remote resource (optional)</param>
        /// <param name="name">File name</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [DisableFormValueModelBinding]
        [Authorize(PlatformConstants.Security.Permissions.AssetCreate)]
        [UploadFile]
        public async Task<ActionResult<BlobInfo[]>> UploadAssetAsync([FromQuery] string folderUrl, [FromQuery] string url = null, [FromQuery] string name = null)
        {
            // https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-8.0
            if (url == null && !MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }

            var result = new List<BlobInfo>();
            try
            {
                if (url != null)
                {
                    var fileName = name ?? HttpUtility.UrlDecode(Path.GetFileName(url));
                    var fileUrl = UrlHelpers.Combine(folderUrl ?? "", Uri.EscapeDataString(fileName));
                    using var client = _httpClientFactory.CreateClient();
                    await using var remoteStream = await client.GetStreamAsync(url);
                    await using var blobStream = await _blobProvider.OpenWriteAsync(fileUrl);

                    await remoteStream.CopyToAsync(blobStream);
                    var blobInfo = AbstractTypeFactory<BlobInfo>.TryCreateInstance();
                    blobInfo.Name = fileName;
                    blobInfo.RelativeUrl = fileUrl;
                    blobInfo.Url = _urlResolver.GetAbsoluteUrl(fileUrl);
                    result.Add(blobInfo);
                }
                else
                {
                    var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), _defaultFormOptions.MultipartBoundaryLengthLimit);
                    var reader = new MultipartReader(boundary, HttpContext.Request.Body);

                    var section = await reader.ReadNextSectionAsync();
                    if (section != null)
                    {
                        var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);

                        if (hasContentDispositionHeader && MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                        {
                            var fileName = contentDisposition.FileName.Value;
                            var targetFilePath = UrlHelpers.Combine(folderUrl ?? "", Uri.EscapeDataString(fileName));
                            var rawTargetFilePath = UrlHelpers.Combine(folderUrl ?? "", fileName);

                            await using (var targetStream = await _blobProvider.OpenWriteAsync(targetFilePath))
                            {
                                await section.Body.CopyToAsync(targetStream);
                            }

                            var blobInfo = AbstractTypeFactory<BlobInfo>.TryCreateInstance();
                            blobInfo.Name = fileName;
                            blobInfo.RelativeUrl = targetFilePath;
                            blobInfo.Url = _urlResolver.GetAbsoluteUrl(rawTargetFilePath);
                            blobInfo.ContentType = MimeTypeResolver.ResolveContentType(fileName);
                            result.Add(blobInfo);
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                return new ObjectResult(new { ex.Message }) { StatusCode = (int)ex.StatusCode };
            }
            catch (PlatformException exc)
            {
                return new ObjectResult(new { exc.Message }) { StatusCode = StatusCodes.Status405MethodNotAllowed };
            }

            return Ok(result.ToArray());
        }

        /// <summary>
        /// Delete blobs by urls
        /// </summary>
        /// <param name="urls"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [Authorize(PlatformConstants.Security.Permissions.AssetDelete)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteBlobsAsync([FromQuery] string[] urls)
        {
            if (urls.IsNullOrEmpty())
            {
                return BadRequest("Please, specify at least one asset URL to delete.");
            }

            await _blobProvider.RemoveAsync(urls);
            return NoContent();
        }

        /// <summary>
        /// SearchAsync asset folders and blobs
        /// </summary>
        /// <param name="folderUrl"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [Authorize(PlatformConstants.Security.Permissions.AssetRead)]
        public async Task<ActionResult<BlobEntrySearchResult>> SearchAssetItemsAsync([FromQuery] string folderUrl = null, [FromQuery] string keyword = null)
        {
            var result = await _blobProvider.SearchAsync(folderUrl, keyword);
            return Ok(result);
        }

        /// <summary>
        /// Create new blob folder
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("folder")]
        [Authorize(PlatformConstants.Security.Permissions.AssetCreate)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateBlobFolderAsync([FromBody] BlobFolder folder)
        {
            var validation = await new BlobFolderValidator().ValidateAsync(folder);

            if (!validation.IsValid)
            {
                return BadRequest(new
                {
                    Message = string.Join(" ", validation.Errors.Select(x => x.ErrorMessage)),
                    Errors = validation.Errors
                });
            }

            await _blobProvider.CreateFolderAsync(folder);
            return NoContent();
        }
    }
}
