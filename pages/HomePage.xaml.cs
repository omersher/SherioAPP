using ApiInterface;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SherioAPP.pages
{
    public partial class HomePage : Page
    {
        private readonly ApiService _apiService = new ApiService();
        private int adults = 2;
        private int children = 0;



        public HomePage()
        {
            InitializeComponent();
            Loaded += HomePage_Loaded;

            // תאריכי מינימום
            CheckInPicker.DisplayDateStart = DateTime.Today;
            CheckOutPicker.DisplayDateStart = DateTime.Today.AddDays(1);
            CheckInPicker.SelectedDateChanged += CheckInChanged;
        }

        private async void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            _ = LoadCities();
            UpdateGuestsUI();
            CheckLoginStatus();
        }

        // ================= תאריכים =================

        private void CheckInChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CheckInPicker.SelectedDate != null)
            {
                CheckOutPicker.DisplayDateStart =
                    CheckInPicker.SelectedDate.Value.AddDays(1);

                if (CheckOutPicker.SelectedDate <= CheckInPicker.SelectedDate)
                    CheckOutPicker.SelectedDate = null;
            }
        }

        private void ConfirmDates_Click(object sender, RoutedEventArgs e)
        {
            if (CheckInPicker.SelectedDate == null || CheckOutPicker.SelectedDate == null)
            {
                MessageBox.Show("נא לבחור תאריכי כניסה ויציאה");
                return;
            }

            DatesText.Text =
                $"{CheckInPicker.SelectedDate:dd/MM} - {CheckOutPicker.SelectedDate:dd/MM}";
            DatesText.Foreground = Brushes.Black;
            DatesPopup.IsOpen = false;
        }

        // ================= ערים =================

        private async System.Threading.Tasks.Task LoadCities()
        {
            try
            {
                var cities = await _apiService.GetAllCitiesAsync();
                CitiesList.ItemsSource = cities;
                CitiesList.DisplayMemberPath = "CityName";
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בטעינת ערים: " + ex.Message + "שגיאה פנימית: " + ex.InnerException);
            }
        }

        private void SelectCity_Click(object sender, RoutedEventArgs e)
        {
            CityPopup.IsOpen = true;
        }

        private void CitySelected(object sender, SelectionChangedEventArgs e)
        {
            if (CitiesList.SelectedItems.Count > 0)
            {
                var names = CitiesList.SelectedItems
                    .Cast<City>()
                    .Select(c => c.CityName);

                SelectedCityText.Text = string.Join(", ", names);
                SelectedCityText.Foreground = Brushes.Black;
            }
        }

        // ================= אורחים =================

        private void PlusAdults(object sender, RoutedEventArgs e)
        {
            adults++;
            UpdateGuestsUI();
        }

        private void MinusAdults(object sender, RoutedEventArgs e)
        {
            if (adults > 1)
                adults--;
            UpdateGuestsUI();
        }

        private void PlusChildren(object sender, RoutedEventArgs e)
        {
            children++;
            UpdateGuestsUI();
        }

        private void MinusChildren(object sender, RoutedEventArgs e)
        {
            if (children > 0)
                children--;
            UpdateGuestsUI();
        }

        private void UpdateGuestsUI()
        {
            AdultsCountText.Text = adults.ToString();
            ChildrenCountText.Text = children.ToString();
            GuestsLabelText.Text = $"{adults} מבוגרים, {children} ילדים";
        }

        private void Guests_Click(object sender, RoutedEventArgs e)
        {
            GuestsPopup.IsOpen = true;
        }

        // ================= התחברות =================

        private void CheckLoginStatus()
        {
            if (App.CurrentUser != null)
            {
                LoginBtn.Visibility = Visibility.Collapsed;
                ProfileBtn.Visibility = Visibility.Visible;
                UserNameText.Text = App.CurrentUser.FullName;
            }
            else
            {
                LoginBtn.Visibility = Visibility.Visible;
                ProfileBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Login());
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MyProfile());
        }

        // ================= חיפוש =================

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser == null)
            {
                MessageBox.Show("עליך להתחבר כדי לבצע חיפוש");
                NavigationService.Navigate(new Login());
                return;
            }

            if (CitiesList.SelectedItems.Count == 0)
            {
                CityPopup.IsOpen = true;
                return;
            }

            if (CheckInPicker.SelectedDate == null || CheckOutPicker.SelectedDate == null)
            {
                DatesPopup.IsOpen = true;
                return;
            }

            var selectedCityIds = CitiesList.SelectedItems
                .Cast<City>()
                .Select(c => c.CityName)
                .ToList();

            var hotels = await _apiService.GetAllHotelsAsync();

            var filteredHotels = hotels
                .Where(h => h.City != null &&
                            selectedCityIds.Contains(h.City.CityName))
                .ToList();

            if (filteredHotels.Count == 0)
            {
                MessageBox.Show("לא נמצאו מלונות בערים שנבחרו");
                return;
            }

            NavigationService.Navigate(new ShowResults(filteredHotels));
        }

        private void Dates_Click(object sender, RoutedEventArgs e)
        {
            DatesPopup.IsOpen = true;
        }
    }
}
