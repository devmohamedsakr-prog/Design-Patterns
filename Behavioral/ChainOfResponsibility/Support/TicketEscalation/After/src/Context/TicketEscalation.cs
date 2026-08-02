using System;

namespace TicketEscalation.After.Context
{
    public class SupportTicket
    {
        public string Id { get; set; } = "";
        public string Description { get; set; } = "";
        public int Priority { get; set; } = 1; // 1=Low, 2=Medium, 3=High, 4=Critical
        public string Status { get; set; } = "Open";
        public string ResolvedBy { get; set; } = "";
        public string ResolutionNotes { get; set; } = "";
    }

    public abstract class SupportHandler
    {
        protected SupportHandler _nextHandler;
        protected int _maxPriority;

        public void SetNext(SupportHandler next) => _nextHandler = next;

        public virtual void HandleTicket(SupportTicket ticket)
        {
            if (CanHandle(ticket))
            {
                Resolve(ticket);
            }
            else if (_nextHandler != null)
            {
                Console.WriteLine($"🔄 Escalating ticket to {_nextHandler.GetType().Name}");
                _nextHandler.HandleTicket(ticket);
            }
            else
            {
                Console.WriteLine($"⚠️ Ticket could not be resolved");
                ticket.Status = "Unresolved";
            }
        }

        protected virtual bool CanHandle(SupportTicket ticket) => ticket.Priority <= _maxPriority;

        protected virtual void Resolve(SupportTicket ticket)
        {
            ticket.Status = "Resolved";
            ticket.ResolvedBy = this.GetType().Name;
            Console.WriteLine($"✅ {this.GetType().Name} resolved ticket");
        }
    }

    public class Level1Support : SupportHandler
    {
        public Level1Support() => _maxPriority = 1;

        protected override void Resolve(SupportTicket ticket)
        {
            ticket.ResolutionNotes = "Basic troubleshooting provided";
            base.Resolve(ticket);
        }
    }

    public class Level2Specialist : SupportHandler
    {
        public Level2Specialist() => _maxPriority = 2;

        protected override void Resolve(SupportTicket ticket)
        {
            ticket.ResolutionNotes = "Technical analysis performed";
            base.Resolve(ticket);
        }
    }

    public class Manager : SupportHandler
    {
        public Manager() => _maxPriority = 3;

        protected override void Resolve(SupportTicket ticket)
        {
            ticket.ResolutionNotes = "Managerial escalation completed";
            base.Resolve(ticket);
        }
    }

    public class Director : SupportHandler
    {
        public Director() => _maxPriority = 4;

        protected override void Resolve(SupportTicket ticket)
        {
            ticket.ResolutionNotes = "Executive decision made";
            base.Resolve(ticket);
        }
    }

    public class SupportEscalationChain
    {
        private SupportHandler _firstHandler;

        public SupportEscalationChain()
        {
            var level1 = new Level1Support();
            var level2 = new Level2Specialist();
            var manager = new Manager();
            var director = new Director();

            level1.SetNext(level2);
            level2.SetNext(manager);
            manager.SetNext(director);

            _firstHandler = level1;
        }

        public void ProcessTicket(SupportTicket ticket)
        {
            Console.WriteLine($"\n🎫 Processing support ticket Priority: {ticket.Priority}");
            _firstHandler.HandleTicket(ticket);
        }
    }
}
