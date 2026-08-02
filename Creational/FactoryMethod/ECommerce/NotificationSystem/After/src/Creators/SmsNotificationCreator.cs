using NotificationSystem.After.Abstracts;
using NotificationSystem.After.Handlers;

namespace NotificationSystem.After.Creators
{
    /// <summary>
    /// SMS Notification Creator
    /// Factory method creates SmsNotificationHandler
    /// </summary>
    public class SmsNotificationCreator : NotificationCreator
    {
        protected override INotificationHandler CreateNotificationHandler()
        {
            return new SmsNotificationHandler();
        }
    }
}
