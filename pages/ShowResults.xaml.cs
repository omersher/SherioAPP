using ApiInterface;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace SherioAPP.pages
{

    public partial class ShowResults : Page
    {
        private readonly ApiService _api = new ApiService();
        private DateTime _checkIn;
        private DateTime _checkOut;

        public ShowResults(List<Hotel> hotels, DateTime checkIn, DateTime checkOut)
        {
            InitializeComponent();
            _checkIn = checkIn;
            _checkOut = checkOut;

            LoadAvailableHotels(hotels);
        }

        private async System.Threading.Tasks.Task<bool> IsRoomTypeAvailable(int roomId,int totalUnits,DateTime checkIn, DateTime checkOut)
        {
            var bookings = await _api.GetAllBookingsAsync();

            int bookedCount = bookings.Count(b =>
                b.Room != null &&
                b.Room.Id == roomId &&
                b.Status != "Cancelled" &&
                !(b.EndDate <= checkIn || b.StartDate >= checkOut)
            );

            return bookedCount < totalUnits;
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
                    if (await IsRoomTypeAvailable(room.Id, room.TotalUnits, _checkIn, _checkOut))
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