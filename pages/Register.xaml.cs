using ApiInterface;
using Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SherioAPP.pages
{
    public partial class Register : Page
    {
        ApiService apiClient = new ApiService();

        public Register()
        {
            InitializeComponent();
        }

        private async void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameBox.Text) ||
                string.IsNullOrWhiteSpace(EmailBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordBoxInput.Password))
            {
                MessageBox.Show("נא למלא את כל שדות החובה");
                return;
            }

            try
            {
                User newUser = new User
                {
                    FullName = FullNameBox.Text.Trim(),
                    GuestID = GuestIdBox.Text.Trim(),
                    Email = EmailBox.Text.Trim(),
                    Phone = "",
                    PassHash = PasswordBoxInput.Password
                };

                int result = await apiClient.InsertUserAsync(newUser);

                if (result > 0)
                {
                    MessageBox.Show("נרשמת בהצלחה!");
                    NavigationService?.Navigate(new Login());
                }
                else
                {
                    MessageBox.Show("המשתמש כבר קיים במערכת");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                MessageBox.Show("שגיאה בהתחברות לשרת");
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService != null)
            {
                this.NavigationService.Navigate(new Login());
            }
        }
    }
}