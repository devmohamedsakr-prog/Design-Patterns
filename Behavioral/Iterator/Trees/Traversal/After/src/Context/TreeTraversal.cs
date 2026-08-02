using System;
using System.Collections.Generic;

namespace TreeTraversal.After.Context
{
    public interface IIterator<T>
    {
        bool HasNext();
        T Next();
    }

    public class TreeNode<T>
    {
        public T Data { get; set; }
        public List<TreeNode<T>> Children { get; set; } = new();

        public TreeNode(T data) => Data = data;

        public void AddChild(TreeNode<T> child) => Children.Add(child);
    }

    public class InOrderIterator<T> : IIterator<T>
    {
        private Queue<T> _queue = new();

        public InOrderIterator(TreeNode<T> root)
        {
            InOrderTraverse(root);
        }

        private void InOrderTraverse(TreeNode<T> node)
        {
            if (node == null) return;
            _queue.Enqueue(node.Data);
            foreach (var child in node.Children)
                InOrderTraverse(child);
        }

        public bool HasNext() => _queue.Count > 0;

        public T Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No more nodes");
            return _queue.Dequeue();
        }
    }

    public class PreOrderIterator<T> : IIterator<T>
    {
        private Queue<T> _queue = new();

        public PreOrderIterator(TreeNode<T> root)
        {
            PreOrderTraverse(root);
        }

        private void PreOrderTraverse(TreeNode<T> node)
        {
            if (node == null) return;
            _queue.Enqueue(node.Data);
            foreach (var child in node.Children)
                PreOrderTraverse(child);
        }

        public bool HasNext() => _queue.Count > 0;

        public T Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No more nodes");
            return _queue.Dequeue();
        }
    }

    public class PostOrderIterator<T> : IIterator<T>
    {
        private Queue<T> _queue = new();

        public PostOrderIterator(TreeNode<T> root)
        {
            PostOrderTraverse(root);
        }

        private void PostOrderTraverse(TreeNode<T> node)
        {
            if (node == null) return;
            foreach (var child in node.Children)
                PostOrderTraverse(child);
            _queue.Enqueue(node.Data);
        }

        public bool HasNext() => _queue.Count > 0;

        public T Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No more nodes");
            return _queue.Dequeue();
        }
    }

    public class BreadthFirstIterator<T> : IIterator<T>
    {
        private Queue<T> _result = new();

        public BreadthFirstIterator(TreeNode<T> root)
        {
            var queue = new Queue<TreeNode<T>>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                _result.Enqueue(node.Data);
                foreach (var child in node.Children)
                    queue.Enqueue(child);
            }
        }

        public bool HasNext() => _result.Count > 0;

        public T Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No more nodes");
            return _result.Dequeue();
        }
    }
}
