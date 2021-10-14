using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.AssetsModule.Web.Controllers.Api
{
    [Route("api/assetentries")]
    public class AssetEntryController : Controller
    {
        private readonly ICrudService<AssetEntry> _assetService;
        private readonly ISearchService<AssetEntrySearchCriteria, AssetEntrySearchResult, AssetEntry> _assetSearchService;

        public AssetEntryController(ICrudService<AssetEntry> assetService, ISearchService<AssetEntrySearchCriteria, AssetEntrySearchResult, AssetEntry> assetSearchService)
        {
            _assetService = assetService;
            _assetSearchService = assetSearchService;
        }

        /// <summary>
        /// SearchAsync for AssetEntries by AssetEntrySearchCriteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        [Authorize(PlatformConstants.Security.Permissions.AssetAccess)]
        public async Task<ActionResult<AssetEntrySearchResult>> Search([FromBody]AssetEntrySearchCriteria criteria)
        {
            var result = await _assetSearchService.SearchAsync(criteria);
            return Ok(result);
        }

        /// <summary>
        /// Get asset details by id
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        [Authorize(PlatformConstants.Security.Permissions.AssetRead)]
        public async Task<ActionResult<AssetEntry>> Get([FromQuery]string id)
        {
            var result = await _assetService.GetByIdsAsync(new[] { id });
            if (result?.Any() == true)
            {
                return Ok(result.Single());
            }

            return NotFound();
        }

        /// <summary>
        /// Create / Update asset entry
        /// </summary>
        [HttpPut]
        [Route("")]
        [Authorize(PlatformConstants.Security.Permissions.AssetUpdate)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Update([FromBody]AssetEntry item)
        {
            await _assetService.SaveChangesAsync(new[] { item });
            return NoContent();
        }

        /// <summary>
        /// Delete asset entries by ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [Authorize(PlatformConstants.Security.Permissions.AssetDelete)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Delete([FromQuery] string[] ids)
        {
            await _assetService.DeleteAsync(ids);
            return NoContent();
        }
    }
}
