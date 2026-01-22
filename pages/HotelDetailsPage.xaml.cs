using ApiInterface;
using System.Windows;
using System.Windows.Controls;

namespace SherioAPP.pages
{
    public partial class HotelDetailsPage : Page
    {
        private readonly int _hotelId;
        private readonly ApiService _api = new ApiService();

        public HotelDetailsPage(int hotelId)
        {
            InitializeComponent();
            _hotelId = hotelId;
            Loaded += HotelDetailsPage_Loaded;
        }

        private async void HotelDetailsPage_Loaded(object sender, RoutedEventArgs e)
        {
            var hotel = await _api.GetHotelByIdAsync(_hotelId);
            var rooms = await _api.GetRoomsByHotelIdAsync(_hotelId);

            DataContext = hotel;
            RoomsGrid.ItemsSource = rooms;
        }
    }
}
