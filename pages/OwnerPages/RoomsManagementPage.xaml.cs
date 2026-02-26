using ApiInterface;
using Model;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace SherioAPP.pages.OwnerPages
{
    public partial class RoomsManagementPage : Page
    {
        private readonly ApiService _api = new ApiService();
        private ObservableCollection<Room> _rooms = new();
        private Room? _selectedRoom;

        public RoomsManagementPage()
        {
            InitializeComponent();
            Loaded += RoomsManagementPage_Loaded;
        }

        private async void RoomsManagementPage_Loaded(object sender, RoutedEventArgs e)
        {
            var rooms = await _api.GetRoomsByHotelIdAsync(App.CurrentHotel.Id);
            _rooms = new ObservableCollection<Room>(rooms);
            RoomsGrid.ItemsSource = _rooms;
        }

        private async void OpenImages_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Room room)
            {
                _selectedRoom = room;

                var images = await _api.GetRoomImagesByRoomIdAsync(room.Id);
                PopupImagesGrid.ItemsSource = images;

                ImagesDialogHost.IsOpen = true;
            }
        }

        private async void AddPopupImage_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedRoom == null) return;
            if (string.IsNullOrWhiteSpace(PopupUrlBox.Text)) return;

            var dto = new RoomImageInsertDto
            {
                RoomId = _selectedRoom.Id,
                ImageUrl = PopupUrlBox.Text
            };

            await _api.InsertRoomImageAsync(dto);

            PopupUrlBox.Text = "";

            var images = await _api.GetRoomImagesByRoomIdAsync(_selectedRoom.Id);
            PopupImagesGrid.ItemsSource = images;
        }

        private async void DeletePopupImage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                await _api.DeleteRoomImageAsync(id);

                if (_selectedRoom != null)
                {
                    var images = await _api.GetRoomImagesByRoomIdAsync(_selectedRoom.Id);
                    PopupImagesGrid.ItemsSource = images;
                }
            }
        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            ImagesDialogHost.IsOpen = false;
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            foreach (var room in _rooms)
            {
                var dto = new RoomUpdateDto
                {
                    Id = room.Id,
                    RoomName = room.RoomName,
                    AdultRate = room.AdultRate,
                    ChildRate = room.ChildRate,
                    HotelId = App.CurrentHotel.Id
                };

                await _api.UpdateRoomAsync(dto);
            }

            MessageBox.Show("נשמר בהצלחה");
        }
    }
}