using ApiInterface;
using Model;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SherioAPP.pages.OwnerPages
{
    public partial class HotelDetailsPage : Page
    {
        private readonly ApiService _api = new ApiService();
        private readonly Hotel _hotel;

        private string _base64ImageData = null;
        private string _originalImageLink;

        public HotelDetailsPage(Hotel hotel)
        {
            InitializeComponent();

            _hotel = hotel;
            _originalImageLink = hotel.MainHotelImageLink;

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
            if (string.IsNullOrEmpty(imageUrl))
                return;

            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imageUrl, UriKind.RelativeOrAbsolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                PreviewImage.Source = bitmap;
            }
            catch
            {
                PreviewImage.Source = null;
            }
        }

        private void UploadBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string filePath = dialog.FileName;
                    FileNameLabel.Text = Path.GetFileName(filePath);

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(filePath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    PreviewImage.Source = bitmap;

                    byte[] imageBytes = File.ReadAllBytes(filePath);
                    _base64ImageData = Convert.ToBase64String(imageBytes);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("שגיאה בטעינת התמונה: " + ex.Message);
                }
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
                MainHotelImageLink = !string.IsNullOrEmpty(_base64ImageData)
                    ? _base64ImageData
                    : _originalImageLink,
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
            HeaderName.Text = NameBox.Text;
            ImageOverlayName.Text = NameBox.Text;
        }
    }
}