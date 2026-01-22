using Model;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SherioAPP.pages
{
    public partial class ShowResults : Page
    {
        public ShowResults(List<Hotel> hotels)
        {
            InitializeComponent();

            ResultsList.ItemsSource = hotels;
        }

    }
}
