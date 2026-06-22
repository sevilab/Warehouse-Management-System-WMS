using System.Collections.ObjectModel;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Services;
using MaterialDesignThemes.Wpf;
using System.IO;
using System.Text;
using Microsoft.Win32;

namespace WarehouseManagementSystem.ViewModels
{
    public class SupplierViewModel : BaseViewModel
    {
        private readonly SupplierService _supplierService;
        private List<Supplier> _allSuppliers = new();
        private readonly AuthService _authService;
        private readonly ReportService _reportService; // Added
        public bool IsAdmin => _authService.IsAdmin;
        private readonly ISnackbarMessageQueue _messageQueue;
        private ObservableCollection<Supplier> _suppliers = new();
        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            set => SetProperty(ref _suppliers, value);
        }

        private Supplier? _selectedSupplier;
        public Supplier? SelectedSupplier
        {
            get => _selectedSupplier;
            set => SetProperty(ref _selectedSupplier, value);
        }

        private string _searchKeyword = string.Empty;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                SetProperty(ref _searchKeyword, value);
                ApplyFilters();
            }
        }

        private string _selectedStatusFilter = "Tümü";
        public string SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set
            {
                SetProperty(ref _selectedStatusFilter, value);
                ApplyFilters();
            }
        }

        private string _selectedSortOption = "Ad (A-Z)";
        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                SetProperty(ref _selectedSortOption, value);
                ApplyFilters();
            }
        }

        public List<string> StatusFilters { get; } = new()
        {
            "Tümü", "Aktif", "Pasif"
        };

        public List<string> SortOptions { get; } = new()
        {
            "Ad (A-Z)", "Ad (Z-A)", "En Yeni", "En Eski"
        };

        private int _resultCount;
        public int ResultCount
        {
            get => _resultCount;
            set => SetProperty(ref _resultCount, value);
        }

        // Form alanları
        private string _formName = string.Empty;
        public string FormName
        {
            get => _formName;
            set => SetProperty(ref _formName, value);
        }

        private string _formContactPerson = string.Empty;
        public string FormContactPerson
        {
            get => _formContactPerson;
            set => SetProperty(ref _formContactPerson, value);
        }

        private string _formEmail = string.Empty;
        public string FormEmail
        {
            get => _formEmail;
            set => SetProperty(ref _formEmail, value);
        }

        private string _formPhone = string.Empty;
        public string FormPhone
        {
            get => _formPhone;
            set => SetProperty(ref _formPhone, value);
        }

        private string _formAddress = string.Empty;
        public string FormAddress
        {
            get => _formAddress;
            set => SetProperty(ref _formAddress, value);
        }

        private bool _formIsActive = true;
        public bool FormIsActive
        {
            get => _formIsActive;
            set => SetProperty(ref _formIsActive, value);
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private bool _isStatusError;
        public bool IsStatusError
        {
            get => _isStatusError;
            set => SetProperty(ref _isStatusError, value);
        }

        // Komutlar
        public RelayCommand AddSupplierCommand { get; private set; }
        public RelayCommand UpdateSupplierCommand { get; private set; }
        public RelayCommand DeleteSupplierCommand { get; private set; }
        public RelayCommand SelectSupplierCommand { get; private set; }
        public RelayCommand ClearFormCommand { get; private set; }
        public RelayCommand ClearFiltersCommand { get; private set; }
        public RelayCommand ExportCommand { get; private set; } // Added

        public SupplierViewModel(SupplierService supplierService,
                                 AuthService authService,
                                 ReportService reportService, // Added
                                 ISnackbarMessageQueue messageQueue)
        {
            _supplierService = supplierService;
            _authService = authService;
            _reportService = reportService; // Initialized
            _messageQueue = messageQueue;

            AddSupplierCommand = new RelayCommand(_ => AddSupplier(), _ => IsAdmin);
            UpdateSupplierCommand = new RelayCommand(_ => UpdateSupplier(),
                _ => SelectedSupplier != null && IsAdmin);
            DeleteSupplierCommand = new RelayCommand(_ => DeleteSupplier(),
                _ => SelectedSupplier != null && IsAdmin);
            SelectSupplierCommand = new RelayCommand(s =>
            {
                if (s is Supplier supplier)
                {
                    SelectedSupplier = supplier;
                    FormName = supplier.Name;
                    FormContactPerson = supplier.ContactPerson;
                    FormEmail = supplier.Email;
                    FormPhone = supplier.Phone;
                    FormAddress = supplier.Address;
                    FormIsActive = supplier.IsActive;
                    OnPropertyChanged(nameof(FormName));
                    OnPropertyChanged(nameof(FormContactPerson));
                    OnPropertyChanged(nameof(FormEmail));
                    OnPropertyChanged(nameof(FormPhone));
                    OnPropertyChanged(nameof(FormAddress));
                    OnPropertyChanged(nameof(FormIsActive));
                    StatusMessage = string.Empty;
                }
            });
            ClearFormCommand = new RelayCommand(_ => ClearForm());
            ClearFiltersCommand = new RelayCommand(_ => ClearFilters());
            ExportCommand = new RelayCommand(_ => ExportToCsv());

            Refresh();
        }

        private void ExportToCsv()
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "CSV Dosyası (*.csv)|*.csv",
                FileName = $"Tedarikci_Listesi_{DateTime.Now:ddMMyyyy_HHmm}.csv"
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    var csvContent = _reportService.GetSuppliersCsv();
                    File.WriteAllText(saveDialog.FileName, csvContent, Encoding.UTF8);
                    _messageQueue.Enqueue("Tedarikçi raporu başarıyla kaydedildi.");
                }
                catch (Exception ex)
                {
                    _messageQueue.Enqueue("Hata: " + ex.Message);
                }
            }
        }

        public void Refresh()
        {
            _allSuppliers = _supplierService.GetAllSuppliers().ToList();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var filtered = _allSuppliers.AsEnumerable();

            // Arama
            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                var kw = SearchKeyword.ToLower();
                filtered = filtered.Where(s =>
                    s.Name.ToLower().Contains(kw) ||
                    s.ContactPerson.ToLower().Contains(kw) ||
                    s.Email.ToLower().Contains(kw));
            }

            // Durum filtresi
            filtered = SelectedStatusFilter switch
            {
                "Aktif" => filtered.Where(s => s.IsActive),
                "Pasif" => filtered.Where(s => !s.IsActive),
                _ => filtered
            };

            // Sıralama
            filtered = SelectedSortOption switch
            {
                "Ad (A-Z)" => filtered.OrderBy(s => s.Name),
                "Ad (Z-A)" => filtered.OrderByDescending(s => s.Name),
                "En Yeni" => filtered.OrderByDescending(s => s.CreatedAt),
                "En Eski" => filtered.OrderBy(s => s.CreatedAt),
                _ => filtered.OrderBy(s => s.Name)
            };

            var result = filtered.ToList();
            Suppliers = new ObservableCollection<Supplier>(result);
            ResultCount = result.Count;
        }

        private void ClearFilters()
        {
            SearchKeyword = string.Empty;
            SelectedStatusFilter = "Tümü";
            SelectedSortOption = "Ad (A-Z)";
        }

        private void ClearForm()
        {
            FormName = string.Empty;
            FormContactPerson = string.Empty;
            FormEmail = string.Empty;
            FormPhone = string.Empty;
            FormAddress = string.Empty;
            FormIsActive = true;
            SelectedSupplier = null;
            StatusMessage = string.Empty;
        }

        private void AddSupplier()
        {
            if (string.IsNullOrWhiteSpace(FormName))
            { SetError("Firma adı boş olamaz!"); return; }
            if (string.IsNullOrWhiteSpace(FormPhone))
            { SetError("Telefon boş olamaz!"); return; }
            if (FormPhone.Length < 10)
            { SetError("Geçersiz telefon numarası!"); return; }
            if (!string.IsNullOrWhiteSpace(FormEmail) && !FormEmail.Contains("@"))
            { SetError("Geçersiz e-posta adresi!"); return; }

            try
            {
                var supplier = new Supplier(FormName, FormContactPerson,
                    FormEmail, FormPhone, FormAddress);
                if (!FormIsActive) supplier.Deactivate();

                _supplierService.AddSupplier(supplier);
                SetSuccess($"✅ '{FormName}' tedarikçisi eklendi.");
                ClearForm();
                Refresh();
            }
            catch (Exception ex)
            {
                SetError($"Hata: {ex.Message}");
            }
        }

        private void UpdateSupplier()
        {
            if (SelectedSupplier == null) return;
            if (string.IsNullOrWhiteSpace(FormName))
            { SetError("Firma adı boş olamaz!"); return; }
            if (!string.IsNullOrWhiteSpace(FormEmail) && !FormEmail.Contains("@"))
            { SetError("Geçersiz e-posta adresi!"); return; }

            try
            {
                _supplierService.UpdateSupplier(SelectedSupplier.Id,
                    FormName, FormContactPerson,
                    FormEmail, FormPhone, FormAddress, FormIsActive);
                SetSuccess($"✅ '{FormName}' güncellendi.");
                Refresh();
            }
            catch (Exception ex)
            {
                SetError($"Hata: {ex.Message}");
            }
        }

        private void DeleteSupplier()
        {
            if (SelectedSupplier == null) return;
            try
            {
                string name = SelectedSupplier.Name;
                _supplierService.DeleteSupplier(SelectedSupplier.Id);
                SetSuccess($"✅ '{name}' silindi.");
                ClearForm();
                Refresh();
            }
            catch (Exception ex)
            {
                SetError($"Hata: {ex.Message}");
            }
        }

        private void SetError(string message)
        {
            StatusMessage = "❌ " + message;
            IsStatusError = true;
            _messageQueue.Enqueue(message);
        }

        private void SetSuccess(string message)
        {
            StatusMessage = message;
            IsStatusError = false;
            _messageQueue.Enqueue(message);
        }
    }
}