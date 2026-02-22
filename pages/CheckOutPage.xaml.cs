using ApiInterface;
using Model;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SherioAPP.pages
{
    public partial class CheckOutPage : Page
    {
        private readonly Room _room;
        private readonly DateTime _checkIn;
        private readonly DateTime _checkOut;
        private readonly int _adults;
        private readonly int _children;

        public CheckOutPage(Room room, DateTime checkIn, DateTime checkOut, int adults, int children)
        {
            InitializeComponent();

            _room = room;
            _checkIn = checkIn.Date;
            _checkOut = checkOut.Date;
            _adults = adults;
            _children = children;

            Loaded += CheckOutPage_Loaded;
        }

        private void CheckOutPage_Loaded(object sender, RoutedEventArgs e)
        {
            int nights = (_checkOut - _checkIn).Days;

            if (nights <= 0)
            {
                MessageBox.Show("תאריכים לא תקינים.");
                NavigationService.GoBack();
                return;
            }

            CalculatePrice(nights);
        }

        private void CalculatePrice(int nights)
        {
            decimal adultRate = Convert.ToDecimal(_room.AdultRate);
            decimal childRate = Convert.ToDecimal(_room.ChildRate);

            decimal pricePerNight =
                (_adults * adultRate) +
                (_children * childRate);

            decimal totalPrice = pricePerNight * nights;

            NightsText.Text = nights.ToString();
            PricePerNightText.Text = $"₪{pricePerNight:N0}";
            TotalPriceText.Text = $"₪{totalPrice:N0}";
        }

        private void PayBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(
                new PaymentPage(_room, _checkIn, _checkOut, _adults, _children)
            );
        }
    }
}