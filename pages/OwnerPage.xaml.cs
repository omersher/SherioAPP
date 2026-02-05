using System.Windows;
using System.Windows.Controls;

namespace SherioAPP.pages.OwnerPages
{
    public partial class OwnerPage : Page
    {
        public OwnerPage()
        {
            InitializeComponent();

            if (App.CurrentHotel == null)
            {
                MessageBox.Show("לא נבחר מלון");
                return;
            }

            DataContext = App.CurrentHotel;
        }

        private void Rooms_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RoomsManagementPage());
        }

        private void Bookings_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new BookingsManagementPage());
        }

        private void HotelDetails_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HotelDetailsPage(App.CurrentHotel));
        }
    }
}
