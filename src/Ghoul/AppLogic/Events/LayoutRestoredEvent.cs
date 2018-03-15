using PeanutButter.TinyEventAggregator;

namespace Ghoul.AppLogic.Events
{
    public class LayoutRestoredEvent : EventBase<string>
    {
    }

    public class LayoutRestoreStartedEvent : EventBase<string>
    {
    }

    public class LayoutSaveStartedEvent : EventBase<bool>
    {
    }

    public class LayoutSaveCompletedEvent : EventBase<bool>
    {
    }
}