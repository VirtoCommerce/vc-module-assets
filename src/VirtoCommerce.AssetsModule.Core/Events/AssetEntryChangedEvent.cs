using System.Collections.Generic;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.AssetsModule.Core.Events
{
    public class AssetEntryChangedEvent : GenericChangedEntryEvent<AssetEntry>
    {
        public AssetEntryChangedEvent(IEnumerable<GenericChangedEntry<AssetEntry>> changedEntries)
    : base(changedEntries)
        {
        }
    }
}
