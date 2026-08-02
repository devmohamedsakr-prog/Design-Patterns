using NUnit.Framework;
using TreeTraversal.After.Context;

namespace TreeTraversal.After.Tests
{
    [TestFixture]
    public class TreeTraversalTests
    {
        private TreeNode<int> _root;

        [SetUp]
        public void Setup()
        {
            _root = new TreeNode<int>(1);
            _root.AddChild(new TreeNode<int>(2));
            _root.AddChild(new TreeNode<int>(3));
            _root.Children[0].AddChild(new TreeNode<int>(4));
            _root.Children[0].AddChild(new TreeNode<int>(5));
        }

        [Test]
        public void InOrder_HasNext() => Assert.That(new InOrderIterator<int>(_root).HasNext(), Is.True);

        [Test]
        public void InOrder_First()
        {
            var iter = new InOrderIterator<int>(_root);
            var first = iter.Next();
            Assert.That(first, Is.EqualTo(1));
        }

        [Test]
        public void PreOrder_Traversal()
        {
            var iter = new PreOrderIterator<int>(_root);
            int count = 0;
            while (iter.HasNext())
            {
                iter.Next();
                count++;
            }
            Assert.That(count, Is.EqualTo(5));
        }

        [Test]
        public void PostOrder_Last()
        {
            var iter = new PostOrderIterator<int>(_root);
            int val = 0;
            while (iter.HasNext())
                val = iter.Next();
            Assert.That(val, Is.EqualTo(1));
        }

        [Test]
        public void BreadthFirst_Order()
        {
            var iter = new BreadthFirstIterator<int>(_root);
            var first = iter.Next();
            var second = iter.Next();
            Assert.That(first, Is.EqualTo(1));
            Assert.That(second, Is.AnyOf(2, 3));
        }
    }
}
