using Model;
using System.Windows;
using System.Windows.Controls;

namespace SherioAPP.pages.OwnerPages
{
    public partial class HotelDetailsPage : Page
    {
        public HotelDetailsPage(Hotel hotel)
        {
            InitializeComponent();
            DataContext = hotel;
        }

        private void SaveDetails_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("השינויים נשמרו (לוגיקה לשרת בהמשך)");
        }
    }
}
