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

            string email = EmailBox.Text.Trim();
            string password = PasswordBoxInput.Password.Trim();

            // ===== התחברות מנהל (קשיח בקוד) =====
            if (email == "admin@sherio.com" && password == "admin123")
            {
                App.IsAdmin = true;
                App.CurrentUser = new User
                {
                    FullName = "מנהל מערכת",
                    Email = email
                };

                var mainAdmin = Application.Current.MainWindow as MainWindow;
                if (mainAdmin != null)
                    mainAdmin.MainFrame.Navigate(new AdminPage());

                return;
            }

            // ===== התחברות רגילה דרך API =====
            try
            {
                UserList users = await apiClient.GetAllUsersAsync();

                var user = users.FirstOrDefault(u =>
                    u.Email == email &&
                    u.PassHash == password);

                if (user != null)
                {
                    App.IsAdmin = false;
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

        private void Owner_Example(object sender, RoutedEventArgs e)
        {
            EmailBox.Text = "rotem.s@gmail.com";
            PasswordBoxInput.Password = "rotem2711";
        }

        private void Admin_Example(object sender, RoutedEventArgs e)
        {
            EmailBox.Text = "admin@sherio.com";
            PasswordBoxInput.Password = "admin123";
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
