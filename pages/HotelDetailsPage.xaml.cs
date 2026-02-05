using ApiInterface;
using Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace SherioAPP.pages
{
    public partial class HotelDetailsPage : Page
    {
        private readonly int _hotelId;
        private readonly ApiService _api = new ApiService();
        private HotelImagesList _allHotelImages;

        private void OrderRoom_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Room room)
            {
                NavigationService.Navigate(new RoomDetailsPage(room.Id));
            }
        }

        public HotelDetailsPage(int hotelId)
        {
            InitializeComponent();
            _hotelId = hotelId;
            Loaded += HotelDetailsPage_Loaded;
        }

        private async void HotelDetailsPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var hotel = await _api.GetHotelByIdAsync(_hotelId);
                var rooms = await _api.GetRoomsByHotelIdAsync(_hotelId);
                var images = await _api.GetHotelImagesByHotelIdAsync(_hotelId);

                DataContext = hotel;
                RoomsGrid.ItemsSource = rooms;
                _allHotelImages = images;

                LoadImagesToGrid(images);
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בטעינת הנתונים: " + ex.Message);
            }
        }

        private void LoadImagesToGrid(HotelImagesList images)
        {
            if (images == null || images.Count == 0) return;

            MainImage.Source = CreateBitmap(images[0].ImageLink);

            if (images.Count > 1)
                SideImage1.Source = CreateBitmap(images[1].ImageLink);

            if (images.Count > 2)
                SideImage2.Source = CreateBitmap(images[2].ImageLink);

            if (images.Count > 3)
                GalleryText.Text = $"+{images.Count - 3} תמונות נוספות";
            else
                GalleryText.Text = "צפה בגלריה";
        }

        private BitmapImage CreateBitmap(string url)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(url, UriKind.RelativeOrAbsolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        private void OpenGallery_Click(object sender, RoutedEventArgs e)
        {
            if (_allHotelImages != null && _allHotelImages.Count > 0)
            {
                GalleryItemsControl.ItemsSource = _allHotelImages;
                GalleryDialog.IsOpen = true;
            }
        }

        private void RoomsGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                if (parent != null)
                {
                    parent.RaiseEvent(eventArg);
                }
            }
        }
    }
}