using ApiInterface;
using Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SherioAPP.pages.AdminPages
{
    public partial class PaymentsADMIN : Page
    {
        private readonly ApiService _api = new ApiService();
        private ObservableCollection<Payment> _payments = new ObservableCollection<Payment>();

        public PaymentsADMIN()
        {
            InitializeComponent();
            Loaded += PaymentsADMIN_Loaded;
        }

        private async void PaymentsADMIN_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadPaymentsAsync();
        }

        private async Task LoadPaymentsAsync()
        {
            try
            {
                var paymentsFromDb = await _api.GetAllPaymentsAsync();

                if (paymentsFromDb == null)
                {
                    _payments = new ObservableCollection<Payment>();
                    PaymentsGrid.ItemsSource = _payments;
                    return;
                }

                _payments = new ObservableCollection<Payment>(paymentsFromDb);
                PaymentsGrid.ItemsSource = _payments;
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בטעינת התשלומים:\n" + ex.Message);
            }
        }

        private void BackToAdmin_Click(object sender, RoutedEventArgs e)
        {
            var main = Application.Current.MainWindow as MainWindow;

            if (main == null)
                return;

            var nav = main.MainFrame.NavigationService;

            if (nav != null)
            {
                while (nav.CanGoBack)
                    nav.RemoveBackEntry();
            }

            main.MainFrame.Navigate(new AdminPage());
        }

        private async void SaveSinglePayment_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is not Payment payment)
                return;

            try
            {
                PaymentsGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                PaymentsGrid.CommitEdit(DataGridEditingUnit.Row, true);

                int result = await _api.UpdatePaymentAsync(payment);

                if (result > 0)
                    MessageBox.Show("התשלום נשמר בהצלחה.");
                else
                    MessageBox.Show("השמירה נכשלה.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בשמירת התשלום:\n" + ex.Message);
            }
        }

        private async void SaveAllPayments_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PaymentsGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                PaymentsGrid.CommitEdit(DataGridEditingUnit.Row, true);

                foreach (var payment in _payments)
                {
                    int result = await _api.UpdatePaymentAsync(payment);

                    if (result <= 0)
                    {
                        MessageBox.Show("אחד התשלומים לא נשמר.");
                        return;
                    }
                }

                MessageBox.Show("כל התשלומים נשמרו בהצלחה.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בשמירה:\n" + ex.Message);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string query = SearchBox.Text?.Trim().ToLower() ?? "";

                if (string.IsNullOrWhiteSpace(query))
                {
                    PaymentsGrid.ItemsSource = _payments;
                    return;
                }

                var filtered = _payments.Where(p =>
                    p.Id.ToString().Contains(query) ||
                    p.BookingID.ToString().Contains(query) ||
                    p.UserID.ToString().Contains(query) ||
                    p.Amount.ToString().Contains(query) ||
                    (p.PayMethod != null && p.PayMethod.ToLower().Contains(query)) ||
                    p.CreatedAt.ToString("dd/MM/yyyy").Contains(query)
                ).ToList();

                PaymentsGrid.ItemsSource = new ObservableCollection<Payment>(filtered);
            }
            catch
            {
                PaymentsGrid.ItemsSource = _payments;
            }
        }
    }
}
