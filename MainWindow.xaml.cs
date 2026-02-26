using SherioAPP.pages;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SherioAPP
{
    public partial class MainWindow : Window
    {
        private bool _isAdminMode = false;
        public MainWindow()

        {
            InitializeComponent();
            MainFrame.Navigated += MainFrame_Navigated;
            MainFrame.NavigationService.RemoveBackEntry();

            MainFrame.Navigate(new HomePage());
        }

        private void MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            // אם אדמין - לא נוגעים בכלל בכפתורים
            if (_isAdminMode)
                return;

            // אם HomePage - אין חץ
            if (e.Content is pages.HomePage)
                BackButton.Visibility = Visibility.Collapsed;
            else
                BackButton.Visibility = MainFrame.CanGoBack
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        public void SetAdminMode(bool isAdmin)
        {
            _isAdminMode = isAdmin;

            if (isAdmin)
            {
                ExitButton.Visibility = Visibility.Collapsed;
                BackButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                ExitButton.Visibility = Visibility.Visible;
            }
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
                MainFrame.GoBack();
        }
    }
}