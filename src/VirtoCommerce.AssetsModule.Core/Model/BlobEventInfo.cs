using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.AssetsModule.Core.Model
{
    /// <summary>
    /// Used for AssetRemoved and AssetUploade events.
    /// </summary>
    public class BlobEventInfo : Entity
    {
        public string Provider { get; set; }
        public string Uri { get; set; }
    }

}
