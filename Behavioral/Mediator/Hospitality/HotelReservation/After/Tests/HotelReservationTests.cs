using NUnit.Framework;
using HotelReservation.After.Context;
using System;

namespace HotelReservation.After.Tests
{
    [TestFixture]
    public class HotelReservationTests
    {
        private HotelCoordinator _hotel;
        private Guest _guest1, _guest2;
        private HotelRoom _room1, _room2;

        [SetUp]
        public void Setup()
        {
            _hotel = new HotelCoordinator();
            _guest1 = new Guest("John Doe", _hotel);
            _guest2 = new Guest("Jane Smith", _hotel);
            _room1 = new HotelRoom { RoomNumber = "101", Type = "Standard" };
            _room2 = new HotelRoom { RoomNumber = "201", Type = "Deluxe" };
            _hotel.RegisterRoom(_room1);
            _hotel.RegisterRoom(_room2);
        }

        [Test]
        public void RegisterRoom_Success() 
            => Assert.That(_room1.IsAvailable, Is.True);

        [Test]
        public void ReserveRoom_Success()
        {
            _guest1.Reserve("Standard", DateTime.Now, DateTime.Now.AddDays(1));
            Assert.That(_room1.IsAvailable, Is.False);
        }

        [Test]
        public void CheckIn_Success()
        {
            _guest1.Reserve("Standard", DateTime.Now, DateTime.Now.AddDays(1));
            _guest1.CheckIn();
            Assert.That(_room1.IsOccupied, Is.True);
        }

        [Test]
        public void CheckOut_Success()
        {
            _guest1.Reserve("Standard", DateTime.Now, DateTime.Now.AddDays(1));
            _guest1.CheckIn();
            _guest1.CheckOut();
            Assert.That(_room1.IsAvailable, Is.True);
        }

        [Test]
        public void MultipleReservations()
        {
            _guest1.Reserve("Standard", DateTime.Now, DateTime.Now.AddDays(1));
            _guest2.Reserve("Deluxe", DateTime.Now, DateTime.Now.AddDays(2));
            Assert.That(_room1.IsAvailable, Is.False);
            Assert.That(_room2.IsAvailable, Is.False);
        }

        [Test]
        public void ReservationDetails_Correct()
        {
            _guest1.Reserve("Standard", DateTime.Now, DateTime.Now.AddDays(1));
            Assert.That(_room1.Type, Is.EqualTo("Standard"));
        }
    }
}
