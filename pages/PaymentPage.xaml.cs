using ApiInterface;
using Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SherioAPP.pages
{
    public partial class PaymentPage : Page
    {
        private readonly ApiService _api = new ApiService();

        private readonly Room _room;
        private readonly DateTime _checkIn;
        private readonly DateTime _checkOut;
        private readonly int _adults;
        private readonly int _children;

        public PaymentPage(Room room, DateTime start, DateTime end, int adults, int children)
        {
            InitializeComponent();

            _room = room;
            _checkIn = start;
            _checkOut = end;
            _adults = adults;
            _children = children;
        }

        private async void PayButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser == null)
            {
                MessageBox.Show("יש להתחבר לפני ביצוע הזמנה");
                return;
            }

            if (string.IsNullOrWhiteSpace(CardNumber.Text) || CardNumber.Text.Length < 16)
            {
                MessageBox.Show("נא להזין מספר כרטיס תקין");
                return;
            }

            PayButton.IsEnabled = false;
            PayButton.Content = "מעבד תשלום...";

            try
            {
                var booking = new Booking
                {
                    RoomID = _room.Id,
                    StartDate = _checkIn,
                    EndDate = _checkOut,
                    AdultCount = _adults,
                    ChildCount = _children,
                    CreatedAt = DateTime.Now,
                    Status = "Confirmed"
                };

                int result = await _api.InsertBookingAsync(booking);

                if (result > 0)
                {
                    NavigationService.Navigate(new ThankYouPage(result));
                }
                else
                {
                    MessageBox.Show("חלה שגיאה ביצירת ההזמנה.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה: " + ex.Message);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
