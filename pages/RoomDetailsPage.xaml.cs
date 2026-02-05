using ApiInterface;
using Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SherioAPP.pages
{
    public partial class RoomDetailsPage : Page
    {
        private readonly int _roomId;
        private readonly ApiService _api = new ApiService();
        private RoomImagesList _roomImages;

        public RoomDetailsPage(int roomId)
        {
            InitializeComponent();
            _roomId = roomId;
            Loaded += RoomDetailsPage_Loaded;
        }

        private async void RoomDetailsPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var room = await _api.GetRoomByIdAsync(_roomId);
                DataContext = room;

                _roomImages = await _api.GetRoomImagesByRoomIdAsync(_roomId);

                LoadImages();
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בטעינת החדר: " + ex.Message);
            }
        }

        private void LoadImages()
        {
            if (_roomImages == null || _roomImages.Count == 0)
                return;

            MainRoomImage.Source = CreateBitmap(_roomImages[0].ImageLink);

            if (_roomImages.Count > 1)
                SideRoomImage1.Source = CreateBitmap(_roomImages[1].ImageLink);

            if (_roomImages.Count > 2)
                SideRoomImage2.Source = CreateBitmap(_roomImages[2].ImageLink);

            GalleryCountText.Text =
                _roomImages.Count > 3 ? $"+{_roomImages.Count - 3} תמונות" : "כל התמונות";
        }

        private void OpenGallery_Click(object sender, RoutedEventArgs e)
        {
            if (_roomImages == null) return;
            GalleryItemsControl.ItemsSource = _roomImages;
            RoomGalleryDialog.IsOpen = true;
        }

        private void BookRoom_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(
                new CheckOutPage(
                    (Room)DataContext,
                    App.CheckInDate,
                    App.CheckOutDate,
                    App.Adults,
                    App.Children
                )
            );
        }

        private BitmapImage CreateBitmap(string url)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(url, UriKind.RelativeOrAbsolute);
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.EndInit();
            return bmp;
        }
    }
}
