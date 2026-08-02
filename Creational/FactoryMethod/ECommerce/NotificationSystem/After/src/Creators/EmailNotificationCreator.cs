using NotificationSystem.After.Abstracts;
using NotificationSystem.After.Handlers;

namespace NotificationSystem.After.Creators
{
    /// <summary>
    /// Email Notification Creator
    /// Factory method creates EmailNotificationHandler
    /// </summary>
    public class EmailNotificationCreator : NotificationCreator
    {
        protected override INotificationHandler CreateNotificationHandler()
        {
            return new EmailNotificationHandler();
        }
    }
}
