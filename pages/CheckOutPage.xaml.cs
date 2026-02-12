using ApiInterface;
using Model;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation; // נדרש עבור NavigationService

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
            // חישוב לילות
            int nights = (_checkOut - _checkIn).Days;

            if (nights <= 0)
            {
                MessageBox.Show("תאריכי ההזמנה אינם תקינים. מינימום לילה אחד.");
                if (NavigationService.CanGoBack) NavigationService.GoBack();
                return;
            }

            // בדיקת זמינות
            bool available = await IsRoomAvailableAsync();

            if (!available)
            {
                MessageBox.Show("החדר אינו זמין בתאריכים שנבחרו.");
                if (NavigationService.CanGoBack) NavigationService.GoBack();
                return;
            }

            // חישוב והצגת מחיר
            CalculatePrice(nights);
        }

        private async Task<bool> IsRoomAvailableAsync()
        {
            // כאן תוכל להוסיף קריאה אמיתית ל-API לבדיקת זמינות אם קיימת
            return true;
        }

        private void CalculatePrice(int nights)
        {
            // וודא שהשדות AdultRate ו-ChildRate קיימים במודל Room שלך
            int pricePerNight = (_adults * _room.AdultRate) + (_children * _room.ChildRate);
            int totalPrice = pricePerNight * nights;

            NightsText.Text = nights.ToString();
            PricePerNightText.Text = $"₪{pricePerNight:N0}";
            TotalPriceText.Text = $"₪{totalPrice:N0}";
        }

        // הפונקציה שחסרה לך להפעלת הכפתור
        private void PayBtn_Click(object sender, RoutedEventArgs e)
        {
            // ניווט לדף התשלום והעברת כל הנתונים
            NavigationService.Navigate(new PaymentPage(_room, _checkIn, _checkOut, _adults, _children));
        }
    }
}