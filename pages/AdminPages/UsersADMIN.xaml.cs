using ApiInterface;
using Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace SherioAPP.pages.AdminPages
{
    public partial class UsersADMIN : Page
    {
        private readonly ApiService _api = new ApiService();
        private List<User> _allUsers = new();

        public UsersADMIN()
        {
            InitializeComponent();
            Loaded += UsersADMIN_Loaded;
        }

        private async void UsersADMIN_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var users = await _api.GetAllUsersAsync();
            _allUsers = users.ToList();
            UsersGrid.ItemsSource = _allUsers;
        }

        // 🔍 חיפוש לפי שם
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = SearchBox.Text.ToLower();

            var filtered = _allUsers
                .Where(u => u.FullName != null &&
                            u.FullName.ToLower().Contains(text))
                .ToList();

            UsersGrid.ItemsSource = filtered;
        }
    }
}
