using Model;
using System.Configuration;
using System.Data;
using System.Windows;

namespace SherioAPP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Hotel? CurrentHotel { get; set; }
        public static bool IsAdmin { get; set; } = false;
        public static User CurrentUser { get; set; }

        // ===== Search data =====
        public static DateTime CheckInDate { get; set; }
        public static DateTime CheckOutDate { get; set; }
        public static int Adults { get; set; }
        public static int Children { get; set; }
    }

}
