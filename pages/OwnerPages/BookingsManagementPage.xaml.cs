using ApiInterface;
using Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SherioAPP.pages.OwnerPages
{
    public partial class BookingsManagementPage : Page
    {
        private readonly ApiService _api = new ApiService();
        private ObservableCollection<Booking> _bookings = new();

        public BookingsManagementPage()
        {
            InitializeComponent();
            Loaded += BookingsManagementPage_Loaded;
        }

        private async void BookingsManagementPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.CurrentHotel == null)
            {
                MessageBox.Show("לא נבחר מלון");
                return;
            }

            try
            {
                var bookingsFromDb =
                    await _api.GetBookingsByHotelAsync(App.CurrentHotel.Id);

                _bookings = new ObservableCollection<Booking>(bookingsFromDb);
                BookingsGrid.ItemsSource = _bookings;
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בטעינת הזמנות:\n" + ex.Message);
            }
        }

        private async void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BookingsGrid.CommitEdit(DataGridEditingUnit.Row, true);
                BookingsGrid.CommitEdit();

                foreach (var booking in _bookings)
                {
                    if (booking.AdultCount < 1)
                    {
                        MessageBox.Show($"הזמנה {booking.Id}: חייב להיות לפחות מבוגר אחד.");
                        return;
                    }

                    if (booking.ChildCount < 0)
                    {
                        MessageBox.Show($"הזמנה {booking.Id}: מספר ילדים לא תקין.");
                        return;
                    }

                    var dto = new BookingUpdateDto
                    {
                        Id = booking.Id,
                        AdultCount = booking.AdultCount,
                        ChildCount = booking.ChildCount,
                        Status = booking.Status
                    };

                    await _api.UpdateBookingAsync(dto);
                }

                MessageBox.Show("כל ההזמנות עודכנו בהצלחה!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בשמירה:\n" + ex.Message);
            }
        }
    }
}