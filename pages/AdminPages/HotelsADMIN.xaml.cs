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
    public partial class HotelsADMIN : Page
    {
        private readonly ApiService _api = new ApiService();
        private ObservableCollection<Hotel> _hotels = new ObservableCollection<Hotel>();

        public HotelsADMIN()
        {
            InitializeComponent();
            Loaded += HotelsADMIN_Loaded;
        }

        private async void HotelsADMIN_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadHotelsAsync();
        }

        private async Task LoadHotelsAsync()
        {
            try
            {
                var hotelsList = await _api.GetAllHotelsAsync();

                if (hotelsList == null)
                {
                    _hotels = new ObservableCollection<Hotel>();
                    HotelsGrid.ItemsSource = _hotels;
                    return;
                }

                _hotels = new ObservableCollection<Hotel>(hotelsList);
                HotelsGrid.ItemsSource = _hotels;
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בטעינת המלונות:\n" + ex.Message);
            }
        }

        private void BackToAdmin_Click(object sender, RoutedEventArgs e)
        {
            var main = Application.Current.MainWindow as MainWindow;

            if (main != null)
                main.MainFrame.Navigate(new AdminPage());
        }

        private async Task<bool> SaveHotelAsync(Hotel hotel)
        {
            if (hotel == null)
                return false;

            if (string.IsNullOrWhiteSpace(hotel.Name))
            {
                MessageBox.Show("שם המלון לא יכול להיות ריק.");
                return false;
            }

            try
            {
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

                int result = await _api.UpdateHotelAsync(dto);
                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בשמירת המלון:\n" + ex.Message);
                return false;
            }
        }

        private async void SaveSingleHotel_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is not Hotel hotel)
                return;

            try
            {
                HotelsGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                HotelsGrid.CommitEdit(DataGridEditingUnit.Row, true);

                bool success = await SaveHotelAsync(hotel);

                if (success)
                {
                    MessageBox.Show($"השינויים עבור '{hotel.Name}' נשמרו בהצלחה.");
                }
                else
                {
                    MessageBox.Show($"השמירה נכשלה עבור '{hotel.Name}'.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בשמירת המלון:\n" + ex.Message);
            }
        }

        private async void SaveAllHotels_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HotelsGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                HotelsGrid.CommitEdit(DataGridEditingUnit.Row, true);

                foreach (var item in _hotels)
                {
                    bool success = await SaveHotelAsync(item);
                    if (!success)
                    {
                        MessageBox.Show("אחת השמירות נכשלה. התהליך נעצר.");
                        return;
                    }
                }

                MessageBox.Show("כל השינויים נשמרו בהצלחה.");
                await LoadHotelsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בשמירה:\n" + ex.Message);
            }
        }

        private async void DeleteHotel_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is not Hotel hotel)
                return;

            var answer = MessageBox.Show(
                $"האם אתה בטוח שברצונך למחוק את {hotel.Name}?",
                "אישור מחיקה",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (answer != MessageBoxResult.Yes)
                return;

            try
            {
                int result = await _api.DeleteHotelAsync(hotel.Id);

                if (result > 0)
                {
                    _hotels.Remove(hotel);
                    MessageBox.Show("המלון נמחק בהצלחה.");
                }
                else
                {
                    MessageBox.Show("המחיקה נכשלה.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה במחיקה:\n" + ex.Message);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string query = SearchBox.Text?.Trim().ToLower() ?? "";

                if (string.IsNullOrWhiteSpace(query))
                {
                    HotelsGrid.ItemsSource = _hotels;
                    return;
                }

                var filtered = _hotels.Where(h =>
                    (h.Name != null && h.Name.ToLower().Contains(query)) ||
                    (h.StreetAddress != null && h.StreetAddress.ToLower().Contains(query)) ||
                    (h.City != null && h.City.CityName != null && h.City.CityName.ToLower().Contains(query))
                ).ToList();

                HotelsGrid.ItemsSource = new ObservableCollection<Hotel>(filtered);
            }
            catch
            {
                HotelsGrid.ItemsSource = _hotels;
            }
        }
    }
}