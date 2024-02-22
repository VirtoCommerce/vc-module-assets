using System.Collections.Generic;
using VirtoCommerce.AssetsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.AssetsModule.Core.Events;

public class BlobDeletedEvent : GenericChangedEntryEvent<BlobEventInfo>
{
    public BlobDeletedEvent(BlobEventInfo eventData)
        : base([new GenericChangedEntry<BlobEventInfo>(eventData, EntryState.Deleted)])
    {
    }

    public BlobDeletedEvent(IEnumerable<GenericChangedEntry<BlobEventInfo>> changedEntries)
        : base(changedEntries)
    {
    }
}
