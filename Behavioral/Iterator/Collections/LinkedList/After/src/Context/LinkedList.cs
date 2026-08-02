using System;
using System.Collections.Generic;

namespace LinkedList.After.Context
{
    public interface IIterator<T>
    {
        bool HasNext();
        T Next();
    }

    public interface ICollection<T>
    {
        IIterator<T> CreateIterator();
    }

    public class Node<T>
    {
        public T Data { get; set; }
        public Node<T> Next { get; set; }

        public Node(T data) => Data = data;
    }

    public class LinkedListIterator<T> : IIterator<T>
    {
        private Node<T> _current;

        public LinkedListIterator(Node<T> head) => _current = head;

        public bool HasNext() => _current != null;

        public T Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No more elements");
            var data = _current.Data;
            _current = _current.Next;
            return data;
        }
    }

    public class CustomLinkedList<T> : ICollection<T>
    {
        private Node<T> _head;
        private int _count = 0;

        public void Add(T data)
        {
            var newNode = new Node<T>(data);
            if (_head == null)
                _head = newNode;
            else
            {
                var current = _head;
                while (current.Next != null)
                    current = current.Next;
                current.Next = newNode;
            }
            _count++;
            Console.WriteLine($"✓ Added: {data}");
        }

        public IIterator<T> CreateIterator() => new LinkedListIterator<T>(_head);

        public int GetCount() => _count;

        public void Display()
        {
            var iterator = CreateIterator();
            Console.WriteLine("LinkedList Contents: ");
            while (iterator.HasNext())
                Console.WriteLine($"  → {iterator.Next()}");
        }
    }

    public class ReverseLinkedListIterator<T> : IIterator<T>
    {
        private List<T> _data = new();
        private int _position;

        public ReverseLinkedListIterator(Node<T> head)
        {
            var current = head;
            while (current != null)
            {
                _data.Add(current.Data);
                current = current.Next;
            }
            _position = _data.Count - 1;
        }

        public bool HasNext() => _position >= 0;

        public T Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No more elements");
            return _data[_position--];
        }
    }
}
