using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.AssetsModule.Core.Model;

public class BlobEventInfo : Entity
{
    public string Uri { get; set; }
    public string Provider { get; set; }
}
