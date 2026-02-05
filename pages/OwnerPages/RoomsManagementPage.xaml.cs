using ApiInterface;
using Model;
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

            // טעינת חדרים אמיתית מה־DB לפי HotelID
            var roomsFromDb = await _api.GetRoomsByHotelIdAsync(App.CurrentHotel.Id);

            _rooms = new ObservableCollection<Room>(roomsFromDb);
            RoomsGrid.ItemsSource = _rooms;
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var room in _rooms)
                {
                    // חיבור למלון
                    if (room.Hotel == null)
                        room.Hotel = App.CurrentHotel;

                    // ולידציה בסיסית
                    if (string.IsNullOrWhiteSpace(room.RoomName))
                    {
                        MessageBox.Show("יש חדר ללא שם");
                        return;
                    }

                    if (room.AdultRate <= 0)
                    {
                        MessageBox.Show($"מחיר מבוגר לא תקין בחדר {room.RoomName}");
                        return;
                    }

                    if (room.Bedrooms <= 0 || room.Bathrooms <= 0)
                    {
                        MessageBox.Show($"שינה / אמבט לא תקינים בחדר {room.RoomName}");
                        return;
                    }

                    // שליחה לשרת
                    if (room.Id == 0)
                        await _api.InsertRoomAsync(room);
                    else
                        await _api.UpdateRoomAsync(room);
                }

                MessageBox.Show("החדרים נשמרו בהצלחה");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("שגיאה בשמירה:\n" + ex.Message);
            }
        }
    }
}
