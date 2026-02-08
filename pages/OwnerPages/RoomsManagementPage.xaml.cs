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

        public RoomsManagementPage()
        {
            InitializeComponent();
            Loaded += RoomsManagementPage_Loaded;
        }

        private async void RoomsManagementPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.CurrentHotel == null)
            {
                MessageBox.Show("לא נבחר מלון");
                return;
            }

            try
            {
                // ✅ שליפה רק של חדרים למלון הנבחר
                var rooms =
                    await _api.GetRoomsByHotelIdAsync(App.CurrentHotel.Id);

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
                RoomsGrid.CommitEdit(DataGridEditingUnit.Row, true);
                RoomsGrid.CommitEdit();

                foreach (var room in _rooms)
                {
                    if (room == null)
                        continue;

                    if (room.AdultRate < 0)
                    {
                        MessageBox.Show($"מחיר לא תקין בחדר {room.RoomName}");
                        return;
                    }

                    // ✅ DTO בלבד – בלי אובייקט Hotel
                    var dto = new RoomUpdateDto
                    {
                        Id = room.Id,
                        RoomName = room.RoomName,
                        AdultRate = room.AdultRate,
                        ChildRate = room.ChildRate,
                        Bedrooms = room.Bedrooms,
                        Bathrooms = room.Bathrooms,
                        HasKitchen = room.HasKitchen,
                        HasParking = room.HasParking,
                        HasBalcony = room.HasBalcony,
                        HasLivingRoom = room.HasLivingRoom,
                        TotalUnits = room.TotalUnits,
                        HotelId = App.CurrentHotel.Id
                    };

                    await _api.UpdateRoomAsync(dto);
                }

                MessageBox.Show("החדרים עודכנו בהצלחה");
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בשמירה:\n" + ex.Message);
            }
        }
    }
}