using ApiInterface;
using Model;
using System;
using System.Windows;
using System.Windows.Controls;

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

            _room = room ?? throw new ArgumentNullException(nameof(room));
            _checkIn = start.Date;
            _checkOut = end.Date;
            _adults = adults;
            _children = children;
        }

        private async void PayButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser == null)
            {
                MessageBox.Show("יש להתחבר קודם.");
                return;
            }

            if (_checkOut <= _checkIn)
            {
                MessageBox.Show("תאריכים לא תקינים.");
                return;
            }

            if (string.IsNullOrWhiteSpace(CardNumber.Text) || CardNumber.Text.Trim().Length < 16)
            {
                MessageBox.Show("תכניס מספר כרטיס תקין (לפחות 16 ספרות).");
                return;
            }

            PayButton.IsEnabled = false;

            try
            {
                // ==========================
                // חישוב מחיר
                // ==========================
                int nights = (_checkOut - _checkIn).Days;

                decimal totalPrice =
                    (nights * _room.AdultRate * _adults) +
                    (nights * _room.ChildRate * _children);

                // ==========================
                // 1️⃣ יצירת Booking
                // ==========================
                var booking = new Booking
                {
                    UserID = App.CurrentUser.Id,
                    RoomID = _room.Id,
                    StartDate = _checkIn,
                    EndDate = _checkOut,
                    AdultCount = _adults,
                    ChildCount = _children,
                    CreatedAt = DateTime.Now,
                    Status = BookingStatus.Confirmed
                };

                int bookingId = await _api.InsertBookingAsync(booking);

                if (bookingId <= 0)
                {
                    MessageBox.Show("יצירת הזמנה נכשלה.");
                    return;
                }

                // ==========================
                // 2️⃣ יצירת Payment
                // ==========================
                var payment = new Payment
                {
                    UserID = App.CurrentUser.Id,
                    BookingID = bookingId,
                    Amount = totalPrice,
                    PayMethod = "Credit Card",
                    CreatedAt = DateTime.Now
                };

                await _api.InsertPaymentAsync(payment);

                // ==========================
                // 3️⃣ יצירת RoomAvailability
                // ==========================
                var roomAvailability = new RoomAvailability
                {
                    RoomID = _room.Id,
                    StartDate = _checkIn,
                    EndDate = _checkOut
                };

                await _api.InsertRoomAvailabilityAsync(roomAvailability);
                int raId = await _api.InsertRoomAvailabilityAsync(roomAvailability);

                if (raId <= 0)
                {
                    MessageBox.Show("RoomAvailability לא נשמר");
                    return;
                }


                NavigationService.Navigate(new ThankYouPage(bookingId));
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה:\n" + ex.Message);
            }
            finally
            {
                PayButton.IsEnabled = true;
            }
        }
    }
}