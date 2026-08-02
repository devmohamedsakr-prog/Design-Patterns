using NUnit.Framework;
using SocialNotifications.After.Context;

namespace SocialNotifications.After.Tests
{
    [TestFixture]
    public class SocialNotificationsTests
    {
        private User _user, _follower;
        private NotificationCenter _center;
        private Post _post;

        [SetUp]
        public void Setup()
        {
            _user = new User("U1", "Alice");
            _follower = new User("U2", "Bob");
            _center = new NotificationCenter("NotifCenter");
            _user.Subscribe(_center);
            _post = new Post("P1", "Hello World", _user);
        }

        [Test]
        public void Like_Notification() { _post.Like(_follower); Assert.That(_center.Notifications.Count, Is.EqualTo(1)); }

        [Test]
        public void Comment_Notification() { _post.AddComment(_follower, "Great post!"); Assert.That(_center.Notifications.Count, Is.EqualTo(1)); }

        [Test]
        public void Follow_Notification() { _user.NotifyFollow(_follower); Assert.That(_center.Notifications.Count, Is.EqualTo(1)); }

        [Test]
        public void MultipleLikes() { _post.Like(_follower); _post.Like(_follower); Assert.That(_center.Notifications.Count, Is.EqualTo(1)); }
    }
}
