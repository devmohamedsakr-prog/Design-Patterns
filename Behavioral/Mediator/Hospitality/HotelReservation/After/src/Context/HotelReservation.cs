using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelReservation.After.Context
{
    public interface IHotelMediator
    {
        void RegisterRoom(HotelRoom room);
        void ReserveRoom(Guest guest, string roomType, DateTime checkIn, DateTime checkOut);
        void CheckIn(Guest guest);
        void CheckOut(Guest guest);
        void NotifyStaff(string message);
    }

    public class HotelCoordinator : IHotelMediator
    {
        private List<HotelRoom> _rooms = new();
        private Dictionary<Guest, HotelRoom> _guestRoomMapping = new();
        private List<string> _staffNotifications = new();

        public void RegisterRoom(HotelRoom room)
        {
            _rooms.Add(room);
            Console.WriteLine($"🏨 Room {room.RoomNumber} ({room.Type}) registered");
        }

        public void ReserveRoom(Guest guest, string roomType, DateTime checkIn, DateTime checkOut)
        {
            var availableRoom = _rooms.FirstOrDefault(r => r.Type == roomType && r.IsAvailable);
            if (availableRoom != null)
            {
                availableRoom.IsAvailable = false;
                _guestRoomMapping[guest] = availableRoom;
                Console.WriteLine($"✅ Room {availableRoom.RoomNumber} reserved for {guest.Name} ({checkIn:d} - {checkOut:d})");
                NotifyStaff($"Prepare room {availableRoom.RoomNumber} for {guest.Name}");
            }
            else
                Console.WriteLine($"❌ No {roomType} rooms available");
        }

        public void CheckIn(Guest guest)
        {
            if (_guestRoomMapping.TryGetValue(guest, out var room))
            {
                room.IsOccupied = true;
                Console.WriteLine($"🔑 {guest.Name} checked in to room {room.RoomNumber}");
                NotifyStaff($"{guest.Name} checked in - Room {room.RoomNumber}");
            }
        }

        public void CheckOut(Guest guest)
        {
            if (_guestRoomMapping.TryGetValue(guest, out var room))
            {
                room.IsOccupied = false;
                room.IsAvailable = true;
                _guestRoomMapping.Remove(guest);
                Console.WriteLine($"🚪 {guest.Name} checked out from room {room.RoomNumber}");
                NotifyStaff($"Clean room {room.RoomNumber}");
            }
        }

        public void NotifyStaff(string message)
        {
            _staffNotifications.Add(message);
            Console.WriteLine($"👨‍💼 Staff notified: {message}");
        }
    }

    public class HotelRoom
    {
        public string RoomNumber { get; set; } = "";
        public string Type { get; set; } = "";
        public bool IsAvailable { get; set; } = true;
        public bool IsOccupied { get; set; } = false;
    }

    public class Guest
    {
        public string Name { get; set; } = "";
        private IHotelMediator _hotel;

        public Guest(string name, IHotelMediator hotel)
        {
            Name = name;
            _hotel = hotel;
        }

        public void Reserve(string roomType, DateTime checkIn, DateTime checkOut)
            => _hotel.ReserveRoom(this, roomType, checkIn, checkOut);
        public void CheckIn() => _hotel.CheckIn(this);
        public void CheckOut() => _hotel.CheckOut(this);
    }
}
