using NUnit.Framework;
using TicketEscalation.After.Context;

namespace TicketEscalation.After.Tests
{
    [TestFixture]
    public class TicketEscalationTests
    {
        private SupportEscalationChain _chain;

        [SetUp]
        public void Setup() => _chain = new SupportEscalationChain();

        [Test]
        public void Level1Support_LowPriority()
        {
            var ticket = new SupportTicket { Id = "T001", Priority = 1, Description = "Reset password" };
            _chain.ProcessTicket(ticket);
            
            Assert.That(ticket.Status, Is.EqualTo("Resolved"));
            Assert.That(ticket.ResolvedBy, Is.EqualTo("Level1Support"));
        }

        [Test]
        public void Level2Specialist_MediumPriority()
        {
            var ticket = new SupportTicket { Id = "T002", Priority = 2, Description = "Software bug" };
            _chain.ProcessTicket(ticket);
            
            Assert.That(ticket.Status, Is.EqualTo("Resolved"));
            Assert.That(ticket.ResolvedBy, Is.EqualTo("Level2Specialist"));
        }

        [Test]
        public void Manager_HighPriority()
        {
            var ticket = new SupportTicket { Id = "T003", Priority = 3, Description = "System outage" };
            _chain.ProcessTicket(ticket);
            
            Assert.That(ticket.Status, Is.EqualTo("Resolved"));
            Assert.That(ticket.ResolvedBy, Is.EqualTo("Manager"));
        }

        [Test]
        public void Director_CriticalPriority()
        {
            var ticket = new SupportTicket { Id = "T004", Priority = 4, Description = "Database failure" };
            _chain.ProcessTicket(ticket);
            
            Assert.That(ticket.Status, Is.EqualTo("Resolved"));
            Assert.That(ticket.ResolvedBy, Is.EqualTo("Director"));
        }

        [Test]
        public void Escalation_CorrectHandler()
        {
            var ticket = new SupportTicket { Id = "T005", Priority = 3 };
            _chain.ProcessTicket(ticket);
            
            Assert.That(ticket.Status, Is.Not.Empty);
        }
    }
}
