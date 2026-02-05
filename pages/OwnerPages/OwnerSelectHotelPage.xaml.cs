using ApiInterface;
using Model;
using System.Windows;
using System.Windows.Controls;

namespace SherioAPP.pages.OwnerPages
{
    public partial class OwnerSelectHotelPage : Page
    {
        private readonly ApiService _api = new ApiService();

        public OwnerSelectHotelPage()
        {
            InitializeComponent();
            Loaded += OwnerSelectHotelPage_Loaded;
        }

        private async void OwnerSelectHotelPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser == null)
            {
                MessageBox.Show("אין משתמש מחובר");
                return;
            }

            try
            {
                HotelList hotels =
                    await _api.GetHotelsByOwnerIdAsync(App.CurrentUser.Id);

                HotelsListBox.ItemsSource = hotels;

                if (hotels.Count == 0)
                    MessageBox.Show("לא נמצאו מלונות למנהל זה");
            }
            catch
            {
                MessageBox.Show("שגיאה בטעינת המלונות");
            }
        }

        private void SelectHotel_Click(object sender, RoutedEventArgs e)
        {
            if (HotelsListBox.SelectedItem == null)
            {
                MessageBox.Show("נא לבחור מלון");
                return;
            }

            App.CurrentHotel = (Hotel)HotelsListBox.SelectedItem;
            NavigationService.Navigate(new OwnerPage());
        }

        private void HotelsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
