using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SherioAPP.pages
{
    public partial class ThankYouPage : Page
    {
        public ThankYouPage(int bookingId)
        {
            InitializeComponent();

            OrderNumberText.Text = $"#{bookingId}";
        }

        private void BackToHome_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HomePage());
        }
    }
}