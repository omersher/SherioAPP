using Model;
using System.Configuration;
using System.Data;
using System.Windows;

namespace SherioAPP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static User? CurrentUser { get; set; }
    }

}
