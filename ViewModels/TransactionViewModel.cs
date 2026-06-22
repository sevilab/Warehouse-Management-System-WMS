using System.Collections.ObjectModel;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Services;
using MaterialDesignThemes.Wpf;
using System.IO;
using System.Text;
using Microsoft.Win32;

namespace WarehouseManagementSystem.ViewModels
{
    public class TransactionViewModel : BaseViewModel
    {
        private readonly ProductService _productService;
        private readonly ReportService _reportService;
        private readonly AuthService _authService;
        private readonly ISnackbarMessageQueue _messageQueue;
        public bool IsAdmin => _authService.IsAdmin;

        private ObservableCollection<Transaction> _transactions = new();
        public ObservableCollection<Transaction> Transactions
        {
            get => _transactions;
            set => SetProperty(ref _transactions, value);
        }

        private ObservableCollection<Transaction> _recentTransactions = new();
        public ObservableCollection<Transaction> RecentTransactions
        {
            get => _recentTransactions;
            set => SetProperty(ref _recentTransactions, value);
        }

        private DateTime _startDate = DateTime.Today.AddMonths(-1);
        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        private DateTime _endDate = DateTime.Today;
        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        private string _filterType = "Tümü";
        public string FilterType
        {
            get => _filterType;
            set
            {
                SetProperty(ref _filterType, value);
                Refresh();
            }
        }

        private decimal _totalIn;
        public decimal TotalIn
        {
            get => _totalIn;
            set => SetProperty(ref _totalIn, value);
        }

        private decimal _totalOut;
        public decimal TotalOut
        {
            get => _totalOut;
            set => SetProperty(ref _totalOut, value);
        }

        private decimal _netBalance;
        public decimal NetBalance
        {
            get => _netBalance;
            set => SetProperty(ref _netBalance, value);
        }

        public List<string> FilterTypes { get; } = new()
        {
            "Tümü", "Stok Girişi", "Stok Çıkışı", "İade"
        };

        public RelayCommand RefreshCommand { get; private set; }
        public RelayCommand ExportCommand { get; private set; }
        public RelayCommand DeleteTransactionCommand { get; private set; }

        public TransactionViewModel(ProductService productService,
                                    ReportService reportService,
                                    AuthService authService,
                                    ISnackbarMessageQueue messageQueue)
        {
            _productService = productService;
            _reportService = reportService;
            _authService = authService;
            _messageQueue = messageQueue;
            RefreshCommand = new RelayCommand(_ => Refresh());
            ExportCommand = new RelayCommand(_ => ExportToCsv());
            DeleteTransactionCommand = new RelayCommand(p => DeleteTransaction(p as Transaction), _ => IsAdmin);
            Refresh();
        }

        private void ExportToCsv()
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "CSV Dosyası (*.csv)|*.csv",
                FileName = $"Islem_Gecmisi_{DateTime.Now:ddMMyyyy_HHmm}.csv"
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    var csvContent = _reportService.GetTransactionsCsv();
                    File.WriteAllText(saveDialog.FileName, csvContent, Encoding.UTF8);
                    _messageQueue.Enqueue("Rapor başarıyla kaydedildi.");
                }
                catch (Exception ex)
                {
                    _messageQueue.Enqueue("Hata: " + ex.Message);
                }
            }
        }

        private void DeleteTransaction(Transaction? transaction)
        {
            if (transaction == null) return;

            try
            {
                _productService.DeleteTransaction(transaction.Id);
                _messageQueue.Enqueue("İşlem başarıyla silindi ve stok güncellendi.");
                Refresh();
            }
            catch (Exception ex)
            {
                _messageQueue.Enqueue("Hata: " + ex.Message);
            }
        }

        public void Refresh()
        {
            var warehouse = Patterns.WarehouseManager.Instance;
            IEnumerable<Transaction> list;

            list = warehouse.Transactions
                .GetByDateRange(StartDate.Date, EndDate.Date.AddDays(1).AddTicks(-1));

            if (FilterType == "Stok Girişi")
                list = list.Where(t => t.Type == TransactionType.StokGirisi);
            else if (FilterType == "Stok Çıkışı")
                list = list.Where(t => t.Type == TransactionType.StokCikisi);
            else if (FilterType == "İade")
                list = list.Where(t => t.Type == TransactionType.Iade);

            var sorted = list.OrderByDescending(t => t.CreatedAt).ToList();
            Transactions = new ObservableCollection<Transaction>(sorted);

            TotalIn = sorted
                .Where(t => t.Type == TransactionType.StokGirisi)
                .Sum(t => t.TotalPrice);

            TotalOut = sorted
                .Where(t => t.Type == TransactionType.StokCikisi)
                .Sum(t => t.TotalPrice);

            NetBalance = TotalIn - TotalOut;

            // Son 10 işlem başa
            var recent = warehouse.Transactions.GetAll()
                 .OrderByDescending(t => t.CreatedAt)
                 .Take(10)
                 .ToList();
            RecentTransactions = new ObservableCollection<Transaction>(recent);
        }
    }
}
