using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialNotifications.After.Context
{
    public interface IUserActivityObserver
    {
        void OnLike(Post post, User likedBy);
        void OnComment(Post post, Comment comment);
        void OnFollow(User follower);
        string GetName();
    }

    public class User
    {
        public string UserId { get; set; } = "";
        public string Name { get; set; } = "";
        private List<IUserActivityObserver> _observers = new();

        public User(string userId, string name)
        {
            UserId = userId;
            Name = name;
        }

        public void Subscribe(IUserActivityObserver observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
                Console.WriteLine($"  ✓ {observer.GetName()} following {Name}");
            }
        }

        public void NotifyLike(Post post, User likedBy)
        {
            foreach (var obs in _observers.ToList())
                obs.OnLike(post, likedBy);
        }

        public void NotifyComment(Post post, Comment comment)
        {
            foreach (var obs in _observers.ToList())
                obs.OnComment(post, comment);
        }

        public void NotifyFollow(User follower)
        {
            foreach (var obs in _observers.ToList())
                obs.OnFollow(follower);
        }
    }

    public class Post
    {
        public string PostId { get; set; } = "";
        public string Content { get; set; } = "";
        public User Author { get; set; }
        public List<User> Likes { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();

        public Post(string postId, string content, User author)
        {
            PostId = postId;
            Content = content;
            Author = author;
        }

        public void Like(User user)
        {
            if (!Likes.Contains(user))
            {
                Likes.Add(user);
                Author.NotifyLike(this, user);
            }
        }

        public void AddComment(User commenter, string text)
        {
            var comment = new Comment { Author = commenter, Text = text };
            Comments.Add(comment);
            Author.NotifyComment(this, comment);
        }
    }

    public class Comment
    {
        public User Author { get; set; }
        public string Text { get; set; } = "";
    }

    public class NotificationCenter : IUserActivityObserver
    {
        public string CenterName { get; set; }
        public List<string> Notifications { get; set; } = new();

        public NotificationCenter(string name)
        {
            CenterName = name;
        }

        public void OnLike(Post post, User likedBy)
        {
            var notif = $"❤️ {likedBy.Name} liked your post";
            Notifications.Add(notif);
            Console.WriteLine($"    {notif}");
        }

        public void OnComment(Post post, Comment comment)
        {
            var notif = $"💬 {comment.Author.Name} commented: {comment.Text}";
            Notifications.Add(notif);
            Console.WriteLine($"    {notif}");
        }

        public void OnFollow(User follower)
        {
            var notif = $"👤 {follower.Name} followed you";
            Notifications.Add(notif);
            Console.WriteLine($"    {notif}");
        }

        public string GetName() => CenterName;
    }
}
