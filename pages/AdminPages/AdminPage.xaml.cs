using System;
using System.Windows;
using System.Windows.Controls;
using SherioAPP.pages.AdminPages; // וודא שהNamespace הזה תואם למיקום הדפים שלך

namespace SherioAPP.pages
{
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
        }

        // ניווט לדף ניהול משתמשים
        private void Users_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.Navigate(new UsersADMIN());
        }

        // ניווט לדף ניהול מלונות
        private void Hotels_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.Navigate(new HotelsADMIN());
        }

        // ניווט לדף ניהול חדרים
        private void Rooms_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.Navigate(new RoomsADMIN());
        }

        // ניווט לדף ניהול הזמנות
        private void Bookings_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.Navigate(new BookingsADMIN());
        }

        // ניווט לדף ניהול תשלומים
        private void Payments_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.Navigate(new PaymentsADMIN());
        }

        // התנתקות וחזרה לדף הבית
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var main = Application.Current.MainWindow as MainWindow;

            Logout();

            if (main != null)
            {
                main.SetAdminMode(false);

                var nav = main.MainFrame.NavigationService;
                if (nav != null)
                {
                    while (nav.CanGoBack)
                        nav.RemoveBackEntry();
                }

                main.MainFrame.Navigate(new HomePage());
            }
        }

        public static void Logout()
        {
            App.CurrentUser = null;
            App.IsAdmin = false;
            App.CurrentHotel = null;
            App.CheckInDate = DateTime.MinValue;
            App.CheckOutDate = DateTime.MinValue;
            App.Adults = 0;
            App.Children = 0;
            App.SelectedRoomId = 0;
        }
    }
}