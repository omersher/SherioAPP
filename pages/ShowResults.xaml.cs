using ApiInterface;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SherioAPP.pages
{
    public partial class ShowResults : Page
    {
        private readonly ApiService _api = new ApiService();
        private readonly DateTime _checkIn;
        private readonly DateTime _checkOut;

        public ShowResults(List<Hotel> hotels, DateTime checkIn, DateTime checkOut)
        {
            InitializeComponent();
            _checkIn = checkIn.Date;
            _checkOut = checkOut.Date;

            LoadAvailableHotels(hotels);
        }

        // חפיפה נכונה: יש חפיפה אם start < end2 && end > start2
        private static bool Overlaps(DateTime aStart, DateTime aEnd, DateTime bStart, DateTime bEnd)
            => aStart < bEnd && aEnd > bStart;

        private async Task<bool> IsRoomTypeAvailable(Room room, DateTime checkIn, DateTime checkOut)
        {
            // הגנות
            if (room == null) return false;
            if (room.TotalUnits <= 0) return false;

            var bookings = await _api.GetAllBookingsAsync();

            int bookedCount = bookings.Count(b =>
                b.RoomID == room.Id &&
                b.Status != BookingStatus.Cancelled &&
                Overlaps(b.StartDate.Date, b.EndDate.Date, checkIn.Date, checkOut.Date)
            );

            return bookedCount < room.TotalUnits;
        }

        private async void LoadAvailableHotels(List<Hotel> hotels)
        {
            var availableHotels = new List<Hotel>();

            foreach (var hotel in hotels)
            {
                var rooms = await _api.GetRoomsByHotelIdAsync(hotel.Id);

                bool hotelAvailable = false;

                foreach (var room in rooms)
                {
                    if (await IsRoomTypeAvailable(room, _checkIn, _checkOut))
                    {
                        hotelAvailable = true;
                        break;
                    }
                }

                if (hotelAvailable)
                    availableHotels.Add(hotel);
            }

            ResultsList.ItemsSource = availableHotels;
        }
    }
}