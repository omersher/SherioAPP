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
    public partial class UsersADMIN : Page
    {
        private readonly ApiService _api = new ApiService();
        private ObservableCollection<User> _users = new();

        public UsersADMIN()
        {
            InitializeComponent();
            Loaded += UsersADMIN_Loaded;
        }

        private async void UsersADMIN_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadUsers();
        }

        private async Task LoadUsers()
        {
            try
            {
                var usersFromDb = await _api.GetAllUsersAsync();
                _users = new ObservableCollection<User>(usersFromDb);
                UsersGrid.ItemsSource = _users;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading users:\n" + ex.Message);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = SearchBox.Text.ToLower();

            var filtered = _users
                .Where(u => u.FullName != null &&
                            u.FullName.ToLower().Contains(text))
                .ToList();

            UsersGrid.ItemsSource = filtered;
        }

        private async void ToggleOwner_Click(object sender, RoutedEventArgs e)
        {
            var user = (sender as Button)?.DataContext as User;
            if (user == null) return;

            try
            {
                await _api.ToggleOwnerAsync(user.Id);
                await LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating owner:\n" + ex.Message);
            }
        }

        private async void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var user = (sender as Button)?.DataContext as User;
            if (user == null) return;

            try
            {
                await _api.DeleteUserAsync(user.Id);
                await LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting user:\n" + ex.Message);
            }
        }
    }
}