using ApiInterface;
using Model;
using System.Windows;
using System.Windows.Controls;

namespace SherioAPP.pages.OwnerPages
{
    public partial class HotelDetailsPage : Page
    {
        private readonly ApiService _api = new ApiService();
        private readonly Hotel _hotel;

        public HotelDetailsPage(Hotel hotel)
        {
            InitializeComponent();
            _hotel = hotel;

            NameBox.Text = hotel.Name;
            PhoneBox.Text = hotel.PhoneNumber;
            EmailBox.Text = hotel.Email;
            WebsiteBox.Text = hotel.WebSite;
            AddressBox.Text = hotel.StreetAddress;
            ImageBox.Text = hotel.MainHotelImageLink;

            PoolCheck.IsChecked = hotel.HasPool;
            GymCheck.IsChecked = hotel.HasGym;
            RestaurantCheck.IsChecked = hotel.HasRestaurant;
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            var dto = new HotelUpdateDto
            {
                Id = _hotel.Id,
                Name = NameBox.Text,
                PhoneNumber = PhoneBox.Text,
                Email = EmailBox.Text,
                WebSite = WebsiteBox.Text,
                StreetAddress = AddressBox.Text,
                MainHotelImageLink = ImageBox.Text,

                HasPool = PoolCheck.IsChecked == true,
                HasGym = GymCheck.IsChecked == true,
                HasRestaurant = RestaurantCheck.IsChecked == true,

                StarRating = _hotel.StarRating
            };

            try
            {
                await _api.UpdateHotelAsync(dto);
                MessageBox.Show("המלון עודכן בהצלחה");
            }
            catch
            {
                MessageBox.Show("שגיאה בעדכון המלון");
            }
        }
    }
}
