using ApiInterface;
using Model;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SherioAPP.pages
{
    public partial class Login : Page
    {
        ApiService apiClient = new ApiService();

        public Login()
        {
            InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            PasswordErrorText.Visibility = Visibility.Collapsed;

            try
            {
                UserList users = await apiClient.GetAllUsersAsync();

                var user = users.FirstOrDefault(u =>
                    u.Email == EmailBox.Text.Trim() &&
                    u.PassHash == PasswordBoxInput.Password.Trim());

                if (user != null)
                {
                    App.CurrentUser = user;

                    var main = Application.Current.MainWindow as MainWindow;
                    if (main != null)
                        main.MainFrame.Navigate(new HomePage());

                    return;
                }

                PasswordErrorText.Text = "אימייל או סיסמה שגויים";
                PasswordErrorText.Visibility = Visibility.Visible;
            }
            catch
            {
                PasswordErrorText.Text = "שגיאה בהתחברות לשרת";
                PasswordErrorText.Visibility = Visibility.Visible;
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var main = Application.Current.MainWindow as MainWindow;
            if (main != null)
                main.MainFrame.Navigate(new Register());
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            EmailBox.Text = "test@test.com";
            PasswordBoxInput.Password = "1234";
        }
    }
}
