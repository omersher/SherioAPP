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
                MessageBox.Show("You must login before making a booking.");
                return;
            }

            MessageBox.Show(
    "User ID: " + App.CurrentUser.Id + "\n" +
    "Room ID: " + _room.Id
);

            MessageBox.Show(
    "User Exists: " + (await _api.GetUserByIdAsync(App.CurrentUser.Id) != null)
);

            if (string.IsNullOrWhiteSpace(CardNumber.Text) || CardNumber.Text.Length < 16)
            {
                MessageBox.Show("Please enter a valid 16-digit card number.");
                return;
            }

            if (_checkOut <= _checkIn)
            {
                MessageBox.Show("Invalid dates selected.");
                return;
            }

            PayButton.IsEnabled = false;
            PayButton.Content = "Processing payment...";

            try
            {
                int nights = (_checkOut - _checkIn).Days;
                decimal totalPrice = nights * _room.AdultRate;

                // ===== CREATE BOOKING =====
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
                    MessageBox.Show("Booking creation failed.");
                    return;
                }

                var payment = new Payment
                {
                    UserID = App.CurrentUser.Id,
                    BookingID = bookingId,
                    Amount = totalPrice,
                    PayMethod = "Credit Card",
                    CreatedAt = DateTime.Now
                };

                int paymentResult = await _api.InsertPaymentAsync(payment);

                if (paymentResult <= 0)
                {
                    MessageBox.Show("Payment saving failed.");
                    return;
                }
                
                NavigationService.Navigate(new ThankYouPage(bookingId));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Payment error:\n" + ex.Message);
            }
            finally
            {
                PayButton.IsEnabled = true;
                PayButton.Content = "Pay & Finish Booking";
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}