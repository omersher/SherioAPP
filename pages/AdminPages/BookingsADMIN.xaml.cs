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
        private ObservableCollection<Booking> _bookings = new ObservableCollection<Booking>();

        public BookingsADMIN()
        {
            InitializeComponent();
            Loaded += BookingsADMIN_Loaded;
        }

        private void BackToAdmin_Click(object sender, RoutedEventArgs e)
        {
            var main = Application.Current.MainWindow as MainWindow;

            if (main == null)
                return;

            var nav = main.MainFrame.NavigationService;

            if (nav != null)
            {
                while (nav.CanGoBack)
                    nav.RemoveBackEntry();
            }

            main.MainFrame.Navigate(new AdminPage());
        }

        private async void BookingsADMIN_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var bookingsFromDb = await _api.GetAllBookingsAsync();

                if (bookingsFromDb == null)
                {
                    _bookings = new ObservableCollection<Booking>();
                    BookingsGrid.ItemsSource = _bookings;
                    return;
                }

                _bookings = new ObservableCollection<Booking>(bookingsFromDb);
                BookingsGrid.ItemsSource = _bookings;
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בטעינת ההזמנות:\n" + ex.Message);
            }
        }

        private async void SaveSingleBooking_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is not Booking booking)
                return;

            try
            {
                BookingsGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                BookingsGrid.CommitEdit(DataGridEditingUnit.Row, true);

                var dto = new BookingUpdateDto
                {
                    Id = booking.Id,
                    AdultCount = booking.AdultCount,
                    ChildCount = booking.ChildCount,
                    Status = booking.Status
                };

                int result = await _api.UpdateBookingAsync(dto);

                if (result > 0)
                    MessageBox.Show("ההזמנה נשמרה בהצלחה.");
                else
                    MessageBox.Show("השמירה נכשלה.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בשמירת ההזמנה:\n" + ex.Message);
            }
        }

        private async void SaveAllBookings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BookingsGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                BookingsGrid.CommitEdit(DataGridEditingUnit.Row, true);

                foreach (var booking in _bookings)
                {
                    var dto = new BookingUpdateDto
                    {
                        Id = booking.Id,
                        AdultCount = booking.AdultCount,
                        ChildCount = booking.ChildCount,
                        Status = booking.Status
                    };

                    int result = await _api.UpdateBookingAsync(dto);

                    if (result <= 0)
                    {
                        MessageBox.Show("אחת ההזמנות לא נשמרה.");
                        return;
                    }
                }

                MessageBox.Show("כל ההזמנות נשמרו בהצלחה.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בשמירה:\n" + ex.Message);
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
                BookingsGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                BookingsGrid.CommitEdit(DataGridEditingUnit.Row, true);

                booking.Status = BookingStatus.Cancelled;

                var dto = new BookingUpdateDto
                {
                    Id = booking.Id,
                    AdultCount = booking.AdultCount,
                    ChildCount = booking.ChildCount,
                    Status = booking.Status
                };

                int result = await _api.UpdateBookingAsync(dto);

                if (result > 0)
                    MessageBox.Show("ההזמנה בוטלה.");
                else
                    MessageBox.Show("הביטול נכשל.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בביטול ההזמנה:\n" + ex.Message);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = SearchBox.Text?.Trim().ToLower() ?? "";

            if (string.IsNullOrWhiteSpace(query))
            {
                BookingsGrid.ItemsSource = _bookings;
                return;
            }

            var filtered = _bookings.Where(b =>
                b.Id.ToString().Contains(query) ||
                b.UserID.ToString().Contains(query) ||
                b.RoomID.ToString().Contains(query) ||
                b.Status.ToString().ToLower().Contains(query) ||
                b.StartDate.ToString("dd/MM/yyyy").Contains(query) ||
                b.EndDate.ToString("dd/MM/yyyy").Contains(query)
            ).ToList();

            BookingsGrid.ItemsSource = new ObservableCollection<Booking>(filtered);
        }
    }
}