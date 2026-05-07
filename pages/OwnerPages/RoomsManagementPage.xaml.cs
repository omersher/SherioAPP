using ApiInterface;
using Model;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SherioAPP.pages.OwnerPages
{
    public partial class RoomsManagementPage : Page
    {
        private readonly ApiService _api = new ApiService();
        private ObservableCollection<Room> _rooms = new ObservableCollection<Room>();

        public RoomsManagementPage()
        {
            InitializeComponent();
            Loaded += RoomsManagementPage_Loaded;
        }

        private async void RoomsManagementPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadRoomsAsync();
        }

        private async Task LoadRoomsAsync()
        {
            try
            {
                if (App.CurrentHotel == null)
                {
                    MessageBox.Show("לא נבחר מלון.");
                    return;
                }

                HotelNameTextBlock.Text = "מלון נוכחי: " + App.CurrentHotel.Name;

                var rooms = await _api.GetRoomsByHotelIdAsync(App.CurrentHotel.Id);

                if (rooms == null || rooms.Count == 0)
                {
                    _rooms = new ObservableCollection<Room>();
                    RoomsGrid.ItemsSource = _rooms;
                    MessageBox.Show("לא נמצאו חדרים במלון הזה.");
                    return;
                }

                foreach (var room in rooms)
                {
                    if (room.Hotel == null)
                    {
                        room.Hotel = new Hotel();
                    }

                    room.Hotel.Id = App.CurrentHotel.Id;
                    room.Hotel.Name = App.CurrentHotel.Name;
                }

                _rooms = new ObservableCollection<Room>(rooms);
                RoomsGrid.ItemsSource = _rooms;
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בטעינת החדרים:\n" + ex.Message);
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (App.CurrentHotel == null)
                {
                    MessageBox.Show("לא נבחר מלון.");
                    return;
                }

                RoomsGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                RoomsGrid.CommitEdit(DataGridEditingUnit.Row, true);

                foreach (var room in _rooms)
                {
                    if (string.IsNullOrWhiteSpace(room.RoomName))
                    {
                        MessageBox.Show("יש חדר בלי שם.");
                        return;
                    }

                    RoomUpdateDto dto = new RoomUpdateDto
                    {
                        Id = room.Id,
                        HotelId = App.CurrentHotel.Id,
                        RoomName = room.RoomName,
                        AdultRate = room.AdultRate,
                        ChildRate = room.ChildRate,
                        Bedrooms = room.Bedrooms,
                        Bathrooms = room.Bathrooms,
                        HasKitchen = room.HasKitchen,
                        HasParking = room.HasParking,
                        HasBalcony = room.HasBalcony,
                        HasLivingRoom = room.HasLivingRoom,
                        TotalUnits = room.TotalUnits
                    };

                    int result = await _api.UpdateRoomAsync(dto);

                    if (result <= 0)
                    {
                        MessageBox.Show("השמירה נכשלה עבור חדר: " + room.RoomName);
                        return;
                    }
                }

                MessageBox.Show("השינויים נשמרו בהצלחה.");
                await LoadRoomsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בשמירה:\n" + ex.Message);
            }
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            await LoadRoomsAsync();
        }
    }
}