using NUnit.Framework;
using FlightBooking.After.Context;

namespace FlightBooking.After.Tests
{
    [TestFixture]
    public class FlightBookingTests
    {
        private FlightBookingCoordinator _coordinator;
        private Passenger _passenger1, _passenger2;

        [SetUp]
        public void Setup()
        {
            _coordinator = new FlightBookingCoordinator();
            _coordinator.RegisterService(new SeatReservationService());
            _coordinator.RegisterService(new PaymentService());
            _coordinator.RegisterService(new TicketIssuingService());
            _passenger1 = new Passenger("Alice Johnson", _coordinator);
            _passenger2 = new Passenger("Bob Smith", _coordinator);
        }

        [Test]
        public void BookFlight_Success()
        {
            var result = _passenger1.BookFlight("AA100", 299.99m);
            Assert.That(result, Is.True);
        }

        [Test]
        public void SeatReservation()
        {
            var result = _coordinator.ReserveSeat(_passenger1, "AA100");
            Assert.That(result, Is.True);
        }

        [Test]
        public void PaymentProcessing()
        {
            var result = _coordinator.ProcessPayment(_passenger1, 299.99m);
            Assert.That(result, Is.True);
        }

        [Test]
        public void TicketIssuing()
        {
            _coordinator.ReserveSeat(_passenger1, "AA100");
            _coordinator.ProcessPayment(_passenger1, 299.99m);
            var result = _coordinator.IssueTicket(_passenger1, "AA100");
            Assert.That(result, Is.True);
        }

        [Test]
        public void MultiplePassengers()
        {
            var result1 = _passenger1.BookFlight("AA100", 299.99m);
            var result2 = _passenger2.BookFlight("AA100", 299.99m);
            Assert.That(result1 && result2, Is.True);
        }

        [Test]
        public void CancelBooking()
        {
            _passenger1.BookFlight("AA100", 299.99m);
            _passenger1.Cancel();
            Assert.Pass();
        }

        [Test]
        public void PassengerName_Correct()
            => Assert.That(_passenger1.Name, Is.EqualTo("Alice Johnson"));
    }
}
