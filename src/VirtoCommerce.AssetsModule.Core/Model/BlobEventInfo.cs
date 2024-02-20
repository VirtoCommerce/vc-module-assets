using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.AssetsModule.Core.Model
{
    public class BlobEventInfo : Entity
    {
        public string Provider { get; set; }
        public string Uri { get; set; }
    }
}
