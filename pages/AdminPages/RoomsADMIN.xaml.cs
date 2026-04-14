using ApiInterface;
using Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SherioAPP.pages.AdminPages
{
    public partial class RoomsADMIN : Page
    {
        private readonly ApiService _api = new ApiService();
        private ObservableCollection<Room> _rooms = new();

        public RoomsADMIN()
        {
            InitializeComponent();
            Loaded += RoomsADMIN_Loaded;
        }

        private async void RoomsADMIN_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadRoomsAsync();
        }

        private void BackToAdmin_Click(object sender, RoutedEventArgs e)
        {
            var main = Application.Current.MainWindow as MainWindow;

            if (main != null)
                main.MainFrame.Navigate(new AdminPage());
        }

        private async Task LoadRoomsAsync()
        {
            try
            {
                var roomsList = await _api.GetAllRoomsAsync();

                if (roomsList != null)
                {
                    _rooms = new ObservableCollection<Room>(roomsList);
                    RoomsGrid.ItemsSource = _rooms;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בטעינת החדרים:\n" + ex.Message);
            }
        }

        private async void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RoomsGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                RoomsGrid.CommitEdit(DataGridEditingUnit.Row, true);

                var selectedRoom = RoomsGrid.SelectedItem as Room;

                if (selectedRoom == null)
                {
                    MessageBox.Show("אנא בחר שורה שערכת לפני הלחיצה על שמירה.");
                    return;
                }

                if (selectedRoom.Hotel == null)
                {
                    MessageBox.Show("לחדר שנבחר אין מלון משויך.");
                    return;
                }

                var dto = new RoomUpdateDto
                {
                    Id = selectedRoom.Id,
                    HotelId = selectedRoom.Hotel.Id,
                    RoomName = selectedRoom.RoomName,
                    AdultRate = selectedRoom.AdultRate,
                    ChildRate = selectedRoom.ChildRate,
                    Bedrooms = selectedRoom.Bedrooms,
                    Bathrooms = selectedRoom.Bathrooms,
                    HasKitchen = selectedRoom.HasKitchen,
                    HasParking = selectedRoom.HasParking,
                    HasBalcony = selectedRoom.HasBalcony,
                    HasLivingRoom = selectedRoom.HasLivingRoom,
                    TotalUnits = selectedRoom.TotalUnits,
                };

                var result = await _api.UpdateRoomAsync(dto);

                if (result > 0)
                {
                    MessageBox.Show("השינוי נשמר בהצלחה!");
                    await LoadRoomsAsync();
                }
                else
                {
                    MessageBox.Show("לא נשמרו שינויים.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בשמירה:\n" + ex.Message);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = SearchBox.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(query))
            {
                RoomsGrid.ItemsSource = _rooms;
                return;
            }

            var filtered = _rooms
                .Where(r =>
                    (r.RoomName != null && r.RoomName.ToLower().Contains(query)) ||
                    (r.Hotel != null && r.Hotel.Id.ToString().Contains(query)) ||
                    r.Id.ToString().Contains(query))
                .ToList();

            RoomsGrid.ItemsSource = new ObservableCollection<Room>(filtered);
        }
    }
}