using ApiInterface;
using Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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
            LoadHotelData();
        }

        private void LoadHotelData()
        {
            NameBox.Text = _hotel.Name;
            PhoneBox.Text = _hotel.PhoneNumber;
            EmailBox.Text = _hotel.Email;
            WebsiteBox.Text = _hotel.WebSite;
            AddressBox.Text = _hotel.StreetAddress;
            PoolCheck.IsChecked = _hotel.HasPool;
            GymCheck.IsChecked = _hotel.HasGym;
            RestaurantCheck.IsChecked = _hotel.HasRestaurant;

            HeaderName.Text = _hotel.Name;
            ImageOverlayName.Text = _hotel.Name;

            LoadImageFromUrl(_hotel.MainHotelImageLink);
        }

        private void LoadImageFromUrl(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                PreviewImage.Source = null;
                return;
            }

            try
            {
                var uri = new Uri(imageUrl, UriKind.Absolute);
                var bitmap = new BitmapImage();

                bitmap.BeginInit();
                bitmap.UriSource = uri;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmap.EndInit();

                PreviewImage.Source = bitmap;
            }
            catch (Exception ex)
            {
                PreviewImage.Source = null;
                MessageBox.Show("שגיאה בטעינת תמונה:\n" + ex.Message + "\n\nURL:\n" + imageUrl);
            }
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
                MainHotelImageLink = _hotel.MainHotelImageLink,
                HasPool = PoolCheck.IsChecked == true,
                HasGym = GymCheck.IsChecked == true,
                HasRestaurant = RestaurantCheck.IsChecked == true,
                StarRating = _hotel.StarRating
            };

            try
            {
                await _api.UpdateHotelAsync(dto);
                MessageBox.Show("המלון עודכן בהצלחה!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בעדכון: " + ex.Message);
            }
        }

        private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (HeaderName != null)
                HeaderName.Text = NameBox.Text;

            if (ImageOverlayName != null)
                ImageOverlayName.Text = NameBox.Text;
        }
    }
}