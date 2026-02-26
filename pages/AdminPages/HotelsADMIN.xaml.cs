using ApiInterface;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SherioAPP.pages.AdminPages
{
    public partial class HotelsADMIN : Page
    {
        private readonly ApiService _api = new ApiService();
        private ObservableCollection<Hotel> _hotels = new();

        public HotelsADMIN()
        {
            InitializeComponent();
            Loaded += HotelsADMIN_Loaded;
        }

        private async void HotelsADMIN_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadHotelsAsync();
        }

        private void BackToAdmin_Click(object sender, RoutedEventArgs e)
        {
            var main = Application.Current.MainWindow as MainWindow;

            if (main != null)
                main.MainFrame.Navigate(new AdminPage());
        }
        private async Task LoadHotelsAsync()
        {
            try
            {
                var hotelsList = await _api.GetAllHotelsAsync();
                if (hotelsList != null)
                {
                    _hotels = new ObservableCollection<Hotel>(hotelsList);
                    HotelsGrid.ItemsSource = _hotels;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בטעינת המלונות:\n" + ex.Message);
            }
        }

        private async void SaveSingleHotel_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is not Hotel hotel) return;

            try
            {
                HotelsGrid.CommitEdit(DataGridEditingUnit.Row, true);

                var dto = new HotelUpdateDto
                {
                    Id = hotel.Id,
                    Name = hotel.Name,
                    PhoneNumber = hotel.PhoneNumber,
                    Email = hotel.Email,
                    WebSite = hotel.WebSite,
                    StreetAddress = hotel.StreetAddress,
                    StarRating = hotel.StarRating,
                    HasPool = hotel.HasPool,
                    HasGym = hotel.HasGym,
                    HasRestaurant = hotel.HasRestaurant,
                    MainHotelImageLink = hotel.MainHotelImageLink
                };

                // תיקון: המרה מפורשת מ-int ללוגיקת הצלחה
                var result = await _api.UpdateHotelAsync(dto);

                if (result > 0) // אם ה-API מחזיר מספר שורות או קוד 200
                {
                    MessageBox.Show($"השינויים עבור '{hotel.Name}' נשמרו בהצלחה!", "עדכון בוצע", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בשמירת המלון:\n{ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteHotel_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is not Hotel hotel) return;

            var result = MessageBox.Show($"האם אתה בטוח שברצונך למחוק את {hotel.Name}?", "אישור מחיקה", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // תיקון: המרה מפורשת מ-int ללוגיקת הצלחה
                    var deleteResult = await _api.DeleteHotelAsync(hotel.Id);

                    if (deleteResult > 0)
                    {
                        _hotels.Remove(hotel);
                        MessageBox.Show("המלון נמחק בהצלחה.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("שגיאה במחיקה:\n" + ex.Message);
                }
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = SearchBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(query))
            {
                HotelsGrid.ItemsSource = _hotels;
                return;
            }

            var filtered = _hotels.Where(h =>
                h.Name.ToLower().Contains(query) ||
                (h.City != null && h.City.CityName.ToLower().Contains(query))
            ).ToList();

            HotelsGrid.ItemsSource = new ObservableCollection<Hotel>(filtered);
        }
    }
}