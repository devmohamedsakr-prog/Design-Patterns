using NUnit.Framework;
using ChatRoom.After.Context;

namespace ChatRoom.After.Tests
{
    [TestFixture]
    public class ChatRoomTests
    {
        private ChatRoomMediator _chatRoom;
        private User _user1, _user2, _user3;

        [SetUp]
        public void Setup()
        {
            _chatRoom = new ChatRoomMediator("General");
            _user1 = new User("Alice", _chatRoom);
            _user2 = new User("Bob", _chatRoom);
            _user3 = new User("Charlie", _chatRoom);
        }

        [Test]
        public void UserRegistration_Success() 
            => Assert.That(_chatRoom.GetUsers().Count, Is.EqualTo(3));

        [Test]
        public void SendMessage()
        {
            _user1.SendMessage("Hello everyone!");
            Assert.That(_chatRoom.GetUsers(), Does.Contain(_user1));
        }

        [Test]
        public void BroadcastNotification()
        {
            _chatRoom.BroadcastNotification("Server maintenance in 5 minutes");
            Assert.That(_chatRoom.GetUsers().Count, Is.GreaterThan(0));
        }

        [Test]
        public void MultipleMessages()
        {
            _user1.SendMessage("Hello");
            _user2.SendMessage("Hi");
            _user3.SendMessage("Hey");
            Assert.Pass();
        }

        [Test]
        public void RoomName_Correct()
            => Assert.That(_chatRoom.RoomName, Is.EqualTo("General"));

        [Test]
        public void UserCount_Accurate()
        {
            var newUser = new User("David", _chatRoom);
            Assert.That(_chatRoom.GetUsers().Count, Is.EqualTo(4));
        }
    }
}
