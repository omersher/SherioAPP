using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SherioAPP.pages
{
    public partial class MyProfile : Page
    {
        public MyProfile()
        {
            InitializeComponent();
            LoadUserData();
        }

        private void LoadUserData()
        {
            // טעינת הנתונים מהמשתמש המחובר
            if (App.CurrentUser != null)
            {
                FullNameBox.Text = App.CurrentUser.FullName;
                EmailBox.Text = App.CurrentUser.Email;
                PhoneBox.Text = App.CurrentUser.Phone;
            }
        }

        // הפונקציה שהייתה חסרה לך וגרמה לשגיאה
        private void BackToHome_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HomePage());
        }

        private void UpdateProfile_Click(object sender, RoutedEventArgs e)
        {
            // כאן תבוא הלוגיקה לעדכון ב-API בעתיד
            MessageBox.Show($"הפרטים של {FullNameBox.Text} עודכנו בהצלחה!", "עדכון פרופיל");
        }

        private void MyBookings_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MyOrdersPage());
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("האם אתה בטוח שברצונך להתנתק?", "התנתקות", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                App.CurrentUser = null;
                NavigationService.Navigate(new HomePage());
            }
        }
    }
}