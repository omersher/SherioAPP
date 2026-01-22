using Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using SherioAPP.pages;

namespace SherioAPP.userControllers
{
    public partial class HotelFrame : UserControl
    {
        public HotelFrame()
        {
            InitializeComponent();
        }

        private void CheckAvailability_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Hotel hotel)
            {
                var nav = NavigationService.GetNavigationService(this);
                nav?.Navigate(new HotelDetailsPage(hotel.Id));
            }
        }
    }
}
