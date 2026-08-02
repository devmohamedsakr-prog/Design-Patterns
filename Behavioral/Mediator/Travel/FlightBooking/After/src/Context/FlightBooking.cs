using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightBooking.After.Context
{
    public interface IBookingMediator
    {
        void RegisterService(BookingService service);
        bool ReserveSeat(Passenger passenger, string flightId);
        bool ProcessPayment(Passenger passenger, decimal amount);
        bool IssueTicket(Passenger passenger, string flightId);
        void CancelBooking(Passenger passenger);
    }

    public class FlightBookingCoordinator : IBookingMediator
    {
        private List<BookingService> _services = new();
        private Dictionary<Passenger, BookingInfo> _bookings = new();

        public void RegisterService(BookingService service)
        {
            _services.Add(service);
            Console.WriteLine($"📋 {service.ServiceName} registered");
        }

        public bool ReserveSeat(Passenger passenger, string flightId)
        {
            var seatService = _services.OfType<SeatReservationService>().FirstOrDefault();
            if (seatService?.ReserveSeat(flightId) ?? false)
            {
                Console.WriteLine($"✅ Seat reserved for {passenger.Name} on flight {flightId}");
                _bookings[passenger] = new BookingInfo { FlightId = flightId, SeatReserved = true };
                return true;
            }
            return false;
        }

        public bool ProcessPayment(Passenger passenger, decimal amount)
        {
            var paymentService = _services.OfType<PaymentService>().FirstOrDefault();
            if (paymentService?.ProcessPayment(amount) ?? false)
            {
                Console.WriteLine($"💳 Payment of ${amount} processed for {passenger.Name}");
                if (_bookings.TryGetValue(passenger, out var booking))
                    booking.PaymentProcessed = true;
                return true;
            }
            return false;
        }

        public bool IssueTicket(Passenger passenger, string flightId)
        {
            var ticketService = _services.OfType<TicketIssuingService>().FirstOrDefault();
            if (ticketService?.IssueTicket(passenger.Name, flightId) ?? false)
            {
                Console.WriteLine($"🎫 Ticket issued for {passenger.Name}");
                if (_bookings.TryGetValue(passenger, out var booking))
                    booking.TicketIssued = true;
                return true;
            }
            return false;
        }

        public void CancelBooking(Passenger passenger)
        {
            if (_bookings.Remove(passenger))
                Console.WriteLine($"❌ Booking cancelled for {passenger.Name}");
        }
    }

    public class BookingInfo
    {
        public string FlightId { get; set; } = "";
        public bool SeatReserved { get; set; }
        public bool PaymentProcessed { get; set; }
        public bool TicketIssued { get; set; }
    }

    public abstract class BookingService
    {
        public string ServiceName { get; set; } = "";
    }

    public class SeatReservationService : BookingService
    {
        private int _availableSeats = 100;

        public SeatReservationService() => ServiceName = "Seat Reservation";

        public bool ReserveSeat(string flightId)
        {
            if (_availableSeats > 0)
            {
                _availableSeats--;
                return true;
            }
            return false;
        }
    }

    public class PaymentService : BookingService
    {
        public PaymentService() => ServiceName = "Payment Processing";

        public bool ProcessPayment(decimal amount)
        {
            return amount > 0;
        }
    }

    public class TicketIssuingService : BookingService
    {
        public TicketIssuingService() => ServiceName = "Ticket Issuing";

        public bool IssueTicket(string passengerName, string flightId)
        {
            return !string.IsNullOrEmpty(passengerName);
        }
    }

    public class Passenger
    {
        public string Name { get; set; } = "";
        private IBookingMediator _coordinator;

        public Passenger(string name, IBookingMediator coordinator)
        {
            Name = name;
            _coordinator = coordinator;
        }

        public bool BookFlight(string flightId, decimal ticketPrice)
        {
            if (!_coordinator.ReserveSeat(this, flightId))
                return false;
            if (!_coordinator.ProcessPayment(this, ticketPrice))
                return false;
            if (!_coordinator.IssueTicket(this, flightId))
                return false;
            return true;
        }

        public void Cancel() => _coordinator.CancelBooking(this);
    }
}
