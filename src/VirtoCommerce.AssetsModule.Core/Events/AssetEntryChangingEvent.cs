using System.Collections.Generic;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.AssetsModule.Core.Events
{
    public class AssetEntryChangingEvent : GenericChangedEntryEvent<AssetEntry>
    {
        public AssetEntryChangingEvent(IEnumerable<GenericChangedEntry<AssetEntry>> changedEntries)
    : base(changedEntries)
        {
        }
    }
}
