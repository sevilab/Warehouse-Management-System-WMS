using System;
using System.Collections.ObjectModel;
using System.Linq;
using WarehouseManagementSystem.Interfaces;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Patterns;
using WarehouseManagementSystem.Services;
using MaterialDesignThemes.Wpf;

namespace WarehouseManagementSystem.ViewModels
{
    public class LocationViewModel : BaseViewModel
    {
        private readonly LocationService _locationService;
        private readonly ProductService _productService; // Added
        private readonly ISnackbarMessageQueue _messageQueue;

        private ObservableCollection<LocationDisplayItem> _locations = new();
        public ObservableCollection<LocationDisplayItem> Locations
        {
            get => _locations;
            set => SetProperty(ref _locations, value);
        }

        private LocationDisplayItem? _selectedLocationItem;
        public LocationDisplayItem? SelectedLocation
        {
            get => _selectedLocationItem;
            set
            {
                if (SetProperty(ref _selectedLocationItem, value))
                {
                    if (value != null)
                    {
                        EditName = value.Location.Name;
                        EditZone = value.Location.Zone;
                        EditFloorText = value.Location.Floor.ToString();
                        EditDescription = value.Location.Description;
                        EditCapacityText = value.Location.Capacity.ToString();
                    }
                    else
                    {
                        ResetEditFields();
                    }
                    OnPropertyChanged(nameof(IsEditMode));
                }
            }
        }

        private string _editCapacityText = "3";
        public string EditCapacityText
        {
            get => _editCapacityText;
            set
            {
                if (SetProperty(ref _editCapacityText, value))
                {
                    if (int.TryParse(value, out int q))
                    {
                        _editCapacity = q;
                        OnPropertyChanged(nameof(EditCapacity));
                    }
                }
            }
        }
        private int _editCapacity = 3;
        public int EditCapacity
        {
            get => _editCapacity;
            set
            {
                if (SetProperty(ref _editCapacity, value))
                {
                    _editCapacityText = value.ToString();
                    OnPropertyChanged(nameof(EditCapacityText));
                }
            }
        }

        private string _editFloorText = "1";
        public string EditFloorText
        {
            get => _editFloorText;
            set
            {
                if (SetProperty(ref _editFloorText, value))
                {
                    if (int.TryParse(value, out int f))
                    {
                        _editFloor = f;
                        OnPropertyChanged(nameof(EditFloor));
                    }
                }
            }
        }
        private int _editFloor = 1;
        public int EditFloor
        {
            get => _editFloor;
            set
            {
                if (SetProperty(ref _editFloor, value))
                {
                    _editFloorText = value.ToString();
                    OnPropertyChanged(nameof(EditFloorText));
                }
            }
        }

        public string[] AvailableZones => new[] { "Ön", "Orta", "Arka" };
        public int[] AvailableFloors => new[] { 1, 2, 3 };

        private string _editName = string.Empty;
        public string EditName
        {
            get => _editName;
            set => SetProperty(ref _editName, value);
        }

        private string _editZone = string.Empty;
        public string EditZone
        {
            get => _editZone;
            set => SetProperty(ref _editZone, value);
        }

        private string _editDescription = string.Empty;
        public string EditDescription
        {
            get => _editDescription;
            set => SetProperty(ref _editDescription, value);
        }

        public bool IsEditMode => SelectedLocation != null;

        public RelayCommand AddCommand { get; }
        public RelayCommand UpdateCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand ClearCommand { get; }

        public LocationViewModel(LocationService locationService, ProductService productService, ISnackbarMessageQueue messageQueue)
        {
            _locationService = locationService;
            _productService = productService;
            _messageQueue = messageQueue;

            AddCommand = new RelayCommand(_ => AddLocation(), _ => !string.IsNullOrWhiteSpace(EditName));
            UpdateCommand = new RelayCommand(_ => UpdateLocation(), _ => IsEditMode && !string.IsNullOrWhiteSpace(EditName));
            DeleteCommand = new RelayCommand(_ => DeleteLocation(), _ => IsEditMode && SelectedLocation?.IsOccupied == false);
            ClearCommand = new RelayCommand(_ => SelectedLocation = null);

            Refresh();
        }

        private void AddLocation()
        {
            try
            {
                var location = new Location(EditName, EditZone, _editFloor, EditDescription, _editCapacity);
                _locationService.AddLocation(location);
                _messageQueue.Enqueue($"{EditName} lokasyonu başarıyla eklendi.");
                ResetEditFields();
                Refresh();
            }
            catch (Exception ex)
            {
                _messageQueue.Enqueue("Hata: " + ex.Message);
            }
        }

        private void UpdateLocation()
        {
            if (SelectedLocation == null) return;
            try
            {
                _locationService.UpdateLocation(SelectedLocation.Location.Id, EditName, EditZone, _editFloor, EditDescription, _editCapacity);
                _messageQueue.Enqueue("Lokasyon güncellendi.");
                Refresh();
                ResetEditFields();
            }
            catch (Exception ex)
            {
                _messageQueue.Enqueue("Hata: " + ex.Message);
            }
        }

        private void DeleteLocation()
        {
            if (SelectedLocation == null) return;
            if (SelectedLocation.IsOccupied)
            {
                _messageQueue.Enqueue("Hata: Dolu bir lokasyon silinemez. Önce ürünleri başka yere taşıyın.");
                return;
            }

            try
            {
                _locationService.DeleteLocation(SelectedLocation.Location.Id);
                _messageQueue.Enqueue("Lokasyon silindi.");
                SelectedLocation = null;
                Refresh();
            }
            catch (Exception ex)
            {
                _messageQueue.Enqueue("Hata: " + ex.Message);
            }
        }

        private void ResetEditFields()
        {
            EditName = string.Empty;
            EditZone = "Ön";
            EditFloorText = "1";
            EditDescription = string.Empty;
            EditCapacityText = "3";
        }

        public void Refresh()
        {
            var allLocations = _locationService.GetAllLocations();
            var allProducts = _productService.GetProductsWithDetails();

            var displayItems = allLocations.Select(loc => new LocationDisplayItem
            {
                Location = loc,
                Occupants = allProducts.Where(p => p.LocationId == loc.Id).ToList()
            }).ToList();

            Locations = new ObservableCollection<LocationDisplayItem>(displayItems);
        }
    }

    public class LocationDisplayItem : BaseViewModel
    {
        public Location Location { get; set; } = null!;
        public List<Product> Occupants { get; set; } = new();

        public bool IsOccupied => Occupants.Any(p => p.StockQuantity > 0);
        public string Status => IsOccupied ? "DOLU" : "BOŞ";
        public string CapacityInfo => $"{Occupants.Count} / {Location.Capacity}";
        public bool IsFull => Occupants.Count >= Location.Capacity;
        
        public string OccupantInfo => IsOccupied 
            ? string.Join(", ", Occupants.Where(p => p.StockQuantity > 0).Select(p => $"{p.Name} ({p.StockQuantity} {p.Unit})"))
            : "—";
    }
}
