using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatRoom.After.Context
{
    public interface IChatRoomMediator
    {
        void RegisterUser(User user);
        void SendMessage(User sender, string message);
        void BroadcastNotification(string notification);
        List<User> GetUsers();
    }

    public class ChatRoomMediator : IChatRoomMediator
    {
        private List<User> _users = new();
        public string RoomName { get; set; } = "";

        public ChatRoomMediator(string roomName) => RoomName = roomName;

        public void RegisterUser(User user)
        {
            _users.Add(user);
            Console.WriteLine($"👤 {user.Name} joined {RoomName}");
            BroadcastNotification($"{user.Name} joined the chat");
        }

        public void SendMessage(User sender, string message)
        {
            Console.WriteLine($"💬 [{RoomName}] {sender.Name}: {message}");
            foreach (var user in _users.Where(u => u.Name != sender.Name))
                user.ReceiveMessage($"{sender.Name}: {message}");
        }

        public void BroadcastNotification(string notification)
        {
            foreach (var user in _users)
                user.ReceiveNotification(notification);
        }

        public List<User> GetUsers() => _users;
    }

    public class User
    {
        public string Name { get; set; } = "";
        private IChatRoomMediator _chatRoom;

        public User(string name, IChatRoomMediator chatRoom)
        {
            Name = name;
            _chatRoom = chatRoom;
            _chatRoom.RegisterUser(this);
        }

        public void SendMessage(string message) => _chatRoom.SendMessage(this, message);
        public void ReceiveMessage(string message) => Console.WriteLine($"📥 {Name} received: {message}");
        public void ReceiveNotification(string notification) => Console.WriteLine($"ℹ️  {Name} notification: {notification}");
    }
}
