using Model;
using System;
using System.Windows;

namespace SherioAPP
{
    public partial class App : Application
    {
        // ===== Logged user =====
        public static User? CurrentUser { get; set; }

        // ===== Admin flag =====
        public static bool IsAdmin { get; set; } = false;

        // ===== Current selected hotel (for owner management) =====
        public static Hotel? CurrentHotel { get; set; }

        // ===== Booking search data =====
        public static DateTime CheckInDate { get; set; }
        public static DateTime CheckOutDate { get; set; }

        public static int Adults { get; set; }
        public static int Children { get; set; }

        public static int SelectedRoomId { get; set; }
        public static int SelectedHotelId { get; set; }

        public static void Logout()
        {
            CurrentUser = null;
            CurrentHotel = null;
            IsAdmin = false;

            CheckInDate = DateTime.MinValue;
            CheckOutDate = DateTime.MinValue;

            Adults = 0;
            Children = 0;

            SelectedRoomId = 0;
            SelectedHotelId = 0;
        }
    }
}