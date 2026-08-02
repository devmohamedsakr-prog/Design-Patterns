using NotificationSystem.After.Abstracts;
using NotificationSystem.After.Handlers;

namespace NotificationSystem.After.Creators
{
    /// <summary>
    /// Push Notification Creator
    /// Factory method creates PushNotificationHandler
    /// </summary>
    public class PushNotificationCreator : NotificationCreator
    {
        protected override INotificationHandler CreateNotificationHandler()
        {
            return new PushNotificationHandler();
        }
    }
}
