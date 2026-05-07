using ApiInterface;
using Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SherioAPP.pages.AdminPages
{
    public partial class UsersADMIN : Page
    {
        private readonly ApiService _api = new ApiService();
        private ObservableCollection<User> _users = new ObservableCollection<User>();

        public UsersADMIN()
        {
            InitializeComponent();
            Loaded += UsersADMIN_Loaded;
        }

        private async void UsersADMIN_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadUsersAsync();
        }

        private async System.Threading.Tasks.Task LoadUsersAsync()
        {
            try
            {
                var usersFromDb = await _api.GetAllUsersAsync();

                if (usersFromDb == null)
                {
                    _users = new ObservableCollection<User>();
                    UsersGrid.ItemsSource = _users;
                    return;
                }

                _users = new ObservableCollection<User>(usersFromDb);
                UsersGrid.ItemsSource = _users;
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בטעינת המשתמשים:\n" + ex.Message);
            }
        }

        private void BackToAdmin_Click(object sender, RoutedEventArgs e)
        {
            var main = Application.Current.MainWindow as MainWindow;

            if (main == null)
                return;

            var nav = main.MainFrame.NavigationService;

            if (nav != null)
            {
                while (nav.CanGoBack)
                    nav.RemoveBackEntry();
            }

            main.MainFrame.Navigate(new AdminPage());
        }

        private void AddNewUser_Click(object sender, RoutedEventArgs e)
        {
            var newUser = new User
            {
                FullName = "",
                Email = "",
                Phone = "",
                IsOwner = false
            };

            _users.Add(newUser);
            UsersGrid.ItemsSource = _users;
            UsersGrid.SelectedItem = newUser;
            UsersGrid.ScrollIntoView(newUser);
        }

        private async void SaveSingleUser_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is not User user)
                return;

            try
            {
                UsersGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                UsersGrid.CommitEdit(DataGridEditingUnit.Row, true);

                if (!ValidateUser(user))
                    return;

                int result;

                if (user.Id <= 0)
                    result = await _api.InsertUserAsync(user);
                else
                    result = await _api.UpdateUserAsync(user);

                if (result > 0)
                {
                    MessageBox.Show("המשתמש נשמר בהצלחה.");
                    await LoadUsersAsync();
                }
                else
                {
                    MessageBox.Show("השמירה נכשלה.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בשמירת המשתמש:\n" + ex.Message);
            }
        }

        private async void SaveAllUsers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UsersGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                UsersGrid.CommitEdit(DataGridEditingUnit.Row, true);

                foreach (var user in _users.ToList())
                {
                    if (!ValidateUser(user))
                        return;

                    int result;

                    if (user.Id <= 0)
                        result = await _api.InsertUserAsync(user);
                    else
                        result = await _api.UpdateUserAsync(user);

                    if (result <= 0)
                    {
                        MessageBox.Show("אחד המשתמשים לא נשמר.");
                        return;
                    }
                }

                MessageBox.Show("כל המשתמשים נשמרו בהצלחה.");
                await LoadUsersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בשמירה:\n" + ex.Message);
            }
        }

        private async void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is not User user)
                return;

            if (user.Id <= 0)
            {
                _users.Remove(user);
                return;
            }

            var answer = MessageBox.Show(
                $"האם למחוק את {user.FullName}?",
                "אישור מחיקה",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (answer != MessageBoxResult.Yes)
                return;

            try
            {
                int result = await _api.DeleteUserAsync(user.Id);

                if (result > 0)
                {
                    _users.Remove(user);
                    MessageBox.Show("המשתמש נמחק.");
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
                    UsersGrid.ItemsSource = _users;
                    return;
                }

                var filtered = _users.Where(u =>
                    (u.FullName != null && u.FullName.ToLower().Contains(query)) ||
                    (u.Email != null && u.Email.ToLower().Contains(query)) ||
                    (u.Phone != null && u.Phone.ToLower().Contains(query)) ||
                    u.Id.ToString().Contains(query)
                ).ToList();

                UsersGrid.ItemsSource = new ObservableCollection<User>(filtered);
            }
            catch
            {
                UsersGrid.ItemsSource = _users;
            }
        }

        private bool ValidateUser(User user)
        {
            if (user == null)
                return false;

            if (string.IsNullOrWhiteSpace(user.FullName))
            {
                MessageBox.Show("שם מלא הוא שדה חובה.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                MessageBox.Show("אימייל הוא שדה חובה.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(user.Phone))
            {
                MessageBox.Show("טלפון הוא שדה חובה.");
                return false;
            }

            return true;
        }
    }
}