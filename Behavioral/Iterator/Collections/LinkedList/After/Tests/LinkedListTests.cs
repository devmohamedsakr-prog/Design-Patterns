using NUnit.Framework;
using LinkedList.After.Context;

namespace LinkedList.After.Tests
{
    [TestFixture]
    public class LinkedListTests
    {
        private CustomLinkedList<int> _list;

        [SetUp]
        public void Setup()
        {
            _list = new CustomLinkedList<int>();
            _list.Add(10);
            _list.Add(20);
            _list.Add(30);
        }

        [Test]
        public void Add_Success() => Assert.That(_list.GetCount(), Is.EqualTo(3));

        [Test]
        public void Iterator_HasNext() => Assert.That(_list.CreateIterator().HasNext(), Is.True);

        [Test]
        public void Iterator_Next()
        {
            var iterator = _list.CreateIterator();
            var first = iterator.Next();
            Assert.That(first, Is.EqualTo(10));
        }

        [Test]
        public void Iterator_IterateAll()
        {
            var iterator = _list.CreateIterator();
            int count = 0;
            while (iterator.HasNext())
            {
                iterator.Next();
                count++;
            }
            Assert.That(count, Is.EqualTo(3));
        }

        [Test]
        public void MultipleIterators()
        {
            var iter1 = _list.CreateIterator();
            var iter2 = _list.CreateIterator();
            
            var val1 = iter1.Next();
            var val2 = iter2.Next();
            Assert.That(val1, Is.EqualTo(val2));
        }

        [Test]
        public void ReverseIteration()
        {
            var head = new Node<int>(10) { Next = new Node<int>(20) { Next = new Node<int>(30) } };
            var list = new CustomLinkedList<int>();
            var revIterator = new ReverseLinkedListIterator<int>(head);
            var first = revIterator.Next();
            Assert.That(first, Is.EqualTo(30));
        }
    }
}
