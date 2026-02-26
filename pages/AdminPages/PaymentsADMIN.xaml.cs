using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SherioAPP.pages.AdminPages
{
    /// <summary>
    /// Interaction logic for PaymentsADMIN.xaml
    /// </summary>
    public partial class PaymentsADMIN : Page
    {
        public PaymentsADMIN()
        {
            InitializeComponent();
        }

        private void BackToAdmin_Click(object sender, RoutedEventArgs e)
        {
            var main = Application.Current.MainWindow as MainWindow;

            if (main != null)
                main.MainFrame.Navigate(new AdminPage());
        }
    }
}
