using System.Collections.Generic;
using VirtoCommerce.AssetsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.AssetsModule.Core.Events;

public class BlobCreatedEvent : GenericChangedEntryEvent<BlobEventInfo>
{
    public BlobCreatedEvent(BlobEventInfo eventData)
        : base([new GenericChangedEntry<BlobEventInfo>(eventData, EntryState.Added)])
    {
    }

    public BlobCreatedEvent(IEnumerable<GenericChangedEntry<BlobEventInfo>> changedEntries)
        : base(changedEntries)
    {
    }
}
