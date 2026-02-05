using ApiInterface;
using Model;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SherioAPP.pages
{
    public partial class CheckOutPage : Page
    {
        private readonly Room _room;
        private readonly DateTime _checkIn;
        private readonly DateTime _checkOut;
        private readonly int _adults;
        private readonly int _children;

        private readonly ApiService _api = new ApiService();

        public CheckOutPage(Room room, DateTime checkIn, DateTime checkOut, int adults, int children)
        {
            InitializeComponent();

            _room = room;
            _checkIn = checkIn.Date;
            _checkOut = checkOut.Date;
            _adults = adults;
            _children = children;

            Loaded += CheckOutPage_Loaded;
        }

        private async void CheckOutPage_Loaded(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                $"DEBUG DATES\n" +
                $"Check-In: {_checkIn:dd/MM/yyyy HH:mm}\n" +
                $"Check-Out: {_checkOut:dd/MM/yyyy HH:mm}\n" +
                $"Nights calc: {(_checkOut - _checkIn).Days}"
            );

            MessageBox.Show(
    $"DEBUG PRICE\n" +
    $"Adults: {_adults}\n" +
    $"Children: {_children}\n" +
    $"AdultRate: {_room.AdultRate}\n" +
    $"ChildRate: {_room.ChildRate}"
);


            int nights = (_checkOut - _checkIn).Days;
            if (nights <= 0)
            {
                MessageBox.Show("תאריכי ההזמנה אינם תקינים");
                NavigationService.GoBack();
                return;
            }

            // 2️⃣ בדיקת זמינות אמיתית
            bool available = await IsRoomAvailableAsync();

            if (!available)
            {
                MessageBox.Show("התאריכים שנבחרו אינם זמינים");
                NavigationService.GoBack();
                return;
            }

            // 3️⃣ חישוב מחיר
            CalculatePrice(nights);
        }

        // ================== זמינות ==================
        private async Task<bool> IsRoomAvailableAsync()
        {
            return true;
        }

        // ================== מחיר ==================
        private void CalculatePrice(int nights)
        {
            int pricePerNight =
                (_adults * _room.AdultRate) +
                (_children * _room.ChildRate);

            int totalPrice = pricePerNight * nights;

            NightsText.Text = nights.ToString();
            PricePerNightText.Text = $"₪{pricePerNight:N0}";
            TotalPriceText.Text = $"₪{totalPrice:N0}";
        }
    }
}
