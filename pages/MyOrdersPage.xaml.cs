using ApiInterface;
using Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SherioAPP.pages
{
    public partial class MyOrdersPage : Page
    {
        private readonly ApiService _api = new ApiService();
        private ObservableCollection<Booking> _myBookings = new();

        public MyOrdersPage()
        {
            InitializeComponent();
            Loaded += MyOrdersPage_Loaded;
        }

        private async void MyOrdersPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser == null)
            {
                MessageBox.Show("יש להתחבר תחילה.");
                NavigationService.GoBack();
                return;
            }

            try
            {
                var allBookings = await _api.GetAllBookingsAsync();

                var myBookings = allBookings
                    .Where(b => b.UserID == App.CurrentUser.Id)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToList();

                _myBookings = new ObservableCollection<Booking>(myBookings);

                OrdersList.ItemsSource = _myBookings;
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בטעינת ההזמנות:\n" + ex.Message);
            }
        }

        private async void CancelBooking_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button.DataContext is not Booking booking) return;

            var confirm = MessageBox.Show("האם אתה בטוח שברצונך לבטל את ההזמנה?", "אישור ביטול",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                var dto = new BookingUpdateDto
                {
                    Id = booking.Id,
                    AdultCount = booking.AdultCount,
                    ChildCount = booking.ChildCount,
                    Status = BookingStatus.Cancelled
                };

                await _api.UpdateBookingAsync(dto);

                // עדכון האובייקט המקומי
                booking.Status = BookingStatus.Cancelled;

                // ריענון ה-UI כדי שה-DataTrigger יופעל והכפתור ייעלם
                OrdersList.Items.Refresh();

                MessageBox.Show("ההזמנה בוטלה בהצלחה.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בתהליך הביטול: " + ex.Message);
            }
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}