using System;
using System.Collections.Generic;
using System.Linq;

namespace AirTrafficControl.After.Context
{
    /// <summary>
    /// Mediator interface
    /// </summary>
    public interface IAirTrafficMediator
    {
        void RegisterPlane(Plane plane);
        void RequestLanding(Plane plane);
        void RequestTakeoff(Plane plane);
        void NotifyPlanes(string message);
    }

    /// <summary>
    /// Concrete mediator
    /// </summary>
    public class ControlTower : IAirTrafficMediator
    {
        private List<Plane> _planes = new();
        private Queue<Plane> _landingQueue = new();
        private List<string> _runways = new() { "Runway-1", "Runway-2", "Runway-3" };
        private Dictionary<Plane, string> _assignedRunways = new();

        public void RegisterPlane(Plane plane)
        {
            _planes.Add(plane);
            Console.WriteLine($"✈️ {plane.CallSign} registered with control tower");
        }

        public void RequestLanding(Plane plane)
        {
            if (!_planes.Contains(plane)) return;
            _landingQueue.Enqueue(plane);
            Console.WriteLine($"📍 {plane.CallSign} queued for landing (Queue size: {_landingQueue.Count})");
            ProcessLanding();
        }

        public void RequestTakeoff(Plane plane)
        {
            if (!_planes.Contains(plane)) return;
            if (_assignedRunways.ContainsKey(plane))
                _runways.Add(_assignedRunways[plane]);
            Console.WriteLine($"🚀 {plane.CallSign} cleared for takeoff");
            plane.Status = "In Flight";
        }

        public void NotifyPlanes(string message)
        {
            foreach (var plane in _planes)
                plane.ReceiveNotification(message);
        }

        private void ProcessLanding()
        {
            if (_landingQueue.Count > 0 && _runways.Count > 0)
            {
                var plane = _landingQueue.Dequeue();
                var runway = _runways.First();
                _runways.RemoveAt(0);
                _assignedRunways[plane] = runway;
                Console.WriteLine($"✅ {plane.CallSign} assigned to {runway}");
                plane.Status = "Landed";
            }
        }
    }

    /// <summary>
    /// Colleague class
    /// </summary>
    public class Plane
    {
        public string CallSign { get; set; } = "";
        public string Status { get; set; } = "Idle";
        private IAirTrafficMediator _mediator;

        public Plane(string callSign, IAirTrafficMediator mediator)
        {
            CallSign = callSign;
            _mediator = mediator;
            _mediator.RegisterPlane(this);
        }

        public void RequestLanding() => _mediator.RequestLanding(this);
        public void RequestTakeoff() => _mediator.RequestTakeoff(this);
        public void ReceiveNotification(string message) => Console.WriteLine($"📢 {CallSign} received: {message}");
    }
}
