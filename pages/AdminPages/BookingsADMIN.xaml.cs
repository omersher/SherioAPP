using ApiInterface;
using Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SherioAPP.pages.AdminPages
{
    public partial class BookingsADMIN : Page
    {
        private readonly ApiService _api = new ApiService();
        private ObservableCollection<Booking> _bookings = new();

        public BookingsADMIN()
        {
            InitializeComponent();
            Loaded += BookingsADMIN_Loaded;
        }

        private async void BookingsADMIN_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var bookingsFromDb = await _api.GetAllBookingsAsync();
                _bookings = new ObservableCollection<Booking>(bookingsFromDb);
                BookingsGrid.ItemsSource = _bookings;
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בטעינת ההזמנות:\n" + ex.Message);
            }
        }

        private async void ApproveBooking_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is not Booking booking)
                return;

            try
            {
                var dto = new BookingUpdateDto
                {
                    Id = booking.Id,
                    AdultCount = booking.AdultCount,
                    ChildCount = booking.ChildCount,
                    Status = "Confirmed"
                };

                await _api.UpdateBookingAsync(dto);

                booking.Status = "Confirmed";
                BookingsGrid.Items.Refresh();

                MessageBox.Show("ההזמנה אושרה בהצלחה!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בעדכון:\n" + ex.Message);
            }
        }

        private async void CancelBooking_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is not Booking booking)
                return;

            var confirm = MessageBox.Show(
                "האם לבטל את ההזמנה?",
                "אישור",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes)
                return;

            try
            {
                var dto = new BookingUpdateDto
                {
                    Id = booking.Id,
                    AdultCount = booking.AdultCount,
                    ChildCount = booking.ChildCount,
                    Status = "Cancelled"
                };

                await _api.UpdateBookingAsync(dto);

                booking.Status = "Cancelled";
                BookingsGrid.Items.Refresh();

                MessageBox.Show("ההזמנה בוטלה.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בביטול:\n" + ex.Message);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = SearchBox.Text?.ToLower() ?? "";

            if (string.IsNullOrWhiteSpace(query))
            {
                BookingsGrid.ItemsSource = _bookings;
                return;
            }

            var filtered = _bookings.Where(b =>
                (b.Status != null && b.Status.ToLower().Contains(query)) ||
                b.UserID.ToString().Contains(query) ||
                b.RoomID.ToString().Contains(query)
            ).ToList();

            BookingsGrid.ItemsSource =
                new ObservableCollection<Booking>(filtered);
        }
    }
}
