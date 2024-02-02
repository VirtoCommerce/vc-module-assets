using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Settings.Events;

namespace VirtoCommerce.AssetsModule.Core.Services;

public interface IFileExtensionService : IEventHandler<ObjectSettingChangedEvent>
{
    public IList<string> WhiteList { get; }
    public IList<string> BlackList { get; }
    bool IsExtensionAllowed(string extension);
}
