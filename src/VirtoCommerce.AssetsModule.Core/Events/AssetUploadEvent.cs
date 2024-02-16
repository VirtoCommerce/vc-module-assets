using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.AssetsModule.Core.Events
{
    public class AssetsUploadInfo : Entity
    {
        public string Provider { get; set; }
        public string Uri { get; set; }
    }

    public class AssetUploadEvent : GenericChangedEntryEvent<AssetsUploadInfo>
    {
        public AssetUploadEvent(AssetsUploadInfo eventData) :
            base([new GenericChangedEntry<AssetsUploadInfo>(eventData, EntryState.Added)])
        {

        }

        public AssetUploadEvent(IEnumerable<GenericChangedEntry<AssetsUploadInfo>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
