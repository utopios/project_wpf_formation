namespace GestionStock.ViewModels;

/// <summary>
/// ViewModel pour la liste des catégories
/// </summary>
public partial class CategoryListViewModel : NavigableViewModelBase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ObservableCollection<Category> _categories = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EditCategoryCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteCategoryCommand))]
    private Category? _selectedCategory;

    public CategoryListViewModel(
        ICategoryRepository categoryRepository,
        IDialogService dialogService)
    {
        _categoryRepository = categoryRepository;
        _dialogService = dialogService;
    }

    public override void OnNavigatedTo(object? parameter)
    {
        LoadCategoriesCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadCategoriesAsync()
    {
        await ExecuteAsync(async () =>
        {
            var categories = await _categoryRepository.GetWithProductCountAsync();
            Categories = new ObservableCollection<Category>(categories);
        });
    }

    [RelayCommand]
    private async Task AddCategoryAsync()
    {
        var name = await _dialogService.PromptAsync("Nouvelle catégorie", "Nom de la catégorie:");
        if (string.IsNullOrWhiteSpace(name)) return;

        if (await _categoryRepository.NameExistsAsync(name))
        {
            await _dialogService.AlertAsync("Erreur", "Une catégorie avec ce nom existe déjà.");
            return;
        }

        await ExecuteAsync(async () =>
        {
            var category = new Category { Name = name };
            await _categoryRepository.AddAsync(category);
            await LoadCategoriesAsync();
            _dialogService.ShowNotification("Catégorie créée", NotificationType.Success);
        });
    }

    [RelayCommand(CanExecute = nameof(HasSelectedCategory))]
    private async Task EditCategoryAsync()
    {
        if (SelectedCategory == null) return;

        var name = await _dialogService.PromptAsync("Modifier la catégorie", "Nom:", SelectedCategory.Name);
        if (string.IsNullOrWhiteSpace(name) || name == SelectedCategory.Name) return;

        if (await _categoryRepository.NameExistsAsync(name, SelectedCategory.Id))
        {
            await _dialogService.AlertAsync("Erreur", "Une catégorie avec ce nom existe déjà.");
            return;
        }

        await ExecuteAsync(async () =>
        {
            SelectedCategory.Name = name;
            await _categoryRepository.UpdateAsync(SelectedCategory);
            await LoadCategoriesAsync();
            _dialogService.ShowNotification("Catégorie modifiée", NotificationType.Success);
        });
    }

    [RelayCommand(CanExecute = nameof(HasSelectedCategory))]
    private async Task DeleteCategoryAsync()
    {
        if (SelectedCategory == null) return;

        if (SelectedCategory.ProductCount > 0)
        {
            await _dialogService.AlertAsync("Erreur",
                $"Impossible de supprimer cette catégorie car elle contient {SelectedCategory.ProductCount} produit(s).");
            return;
        }

        var confirmed = await _dialogService.ConfirmAsync("Confirmation",
            $"Voulez-vous vraiment supprimer la catégorie '{SelectedCategory.Name}' ?");

        if (confirmed)
        {
            await ExecuteAsync(async () =>
            {
                await _categoryRepository.DeleteAsync(SelectedCategory.Id);
                Categories.Remove(SelectedCategory);
                _dialogService.ShowNotification("Catégorie supprimée", NotificationType.Success);
            });
        }
    }

    private bool HasSelectedCategory() => SelectedCategory != null;
}

/// <summary>
/// ViewModel pour la liste des fournisseurs
/// </summary>
public partial class SupplierListViewModel : NavigableViewModelBase
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ObservableCollection<Supplier> _suppliers = [];

    [ObservableProperty]
    private ObservableCollection<Supplier> _allSuppliers = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedSupplier))]
    [NotifyCanExecuteChangedFor(nameof(EditSupplierCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteSupplierCommand))]
    private Supplier? _selectedSupplier;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _showInactiveOnly;

    public bool HasSelectedSupplier => SelectedSupplier != null;

    public SupplierListViewModel(
        ISupplierRepository supplierRepository,
        INavigationService navigationService,
        IDialogService dialogService)
    {
        _supplierRepository = supplierRepository;
        _navigationService = navigationService;
        _dialogService = dialogService;
    }

    public override void OnNavigatedTo(object? parameter)
    {
        LoadSuppliersCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadSuppliersAsync()
    {
        await ExecuteAsync(async () =>
        {
            var suppliers = await _supplierRepository.GetAllAsync();
            AllSuppliers = new ObservableCollection<Supplier>(suppliers);
            ApplyFilters();
        });
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilters();
    }

    partial void OnShowInactiveOnlyChanged(bool value)
    {
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        var filtered = AllSuppliers.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var term = SearchText.ToLower();
            filtered = filtered.Where(s =>
                s.Name.ToLower().Contains(term) ||
                (s.ContactName != null && s.ContactName.ToLower().Contains(term)) ||
                (s.Email != null && s.Email.ToLower().Contains(term)) ||
                (s.Phone != null && s.Phone.ToLower().Contains(term)));
        }

        if (ShowInactiveOnly)
        {
            filtered = filtered.Where(s => !s.IsActive);
        }

        Suppliers = new ObservableCollection<Supplier>(filtered);
    }

    [RelayCommand]
    private void AddSupplier()
    {
        _navigationService.NavigateTo<SupplierEditViewModel>();
    }

    [RelayCommand(CanExecute = nameof(HasSelectedSupplier))]
    private void EditSupplier()
    {
        if (SelectedSupplier != null)
        {
            _navigationService.NavigateTo<SupplierEditViewModel, int>(SelectedSupplier.Id);
        }
    }

    [RelayCommand(CanExecute = nameof(HasSelectedSupplier))]
    private async Task DeleteSupplierAsync()
    {
        if (SelectedSupplier == null) return;

        if (SelectedSupplier.Products.Any())
        {
            await _dialogService.AlertAsync("Erreur",
                $"Impossible de supprimer ce fournisseur car il est associé à {SelectedSupplier.Products.Count} produit(s).");
            return;
        }

        var confirmed = await _dialogService.ConfirmAsync("Confirmation",
            $"Voulez-vous vraiment supprimer le fournisseur '{SelectedSupplier.Name}' ?");

        if (confirmed)
        {
            await ExecuteAsync(async () =>
            {
                await _supplierRepository.DeleteAsync(SelectedSupplier.Id);
                Suppliers.Remove(SelectedSupplier);
                SelectedSupplier = null;
                _dialogService.ShowNotification("Fournisseur supprimé avec succès", NotificationType.Success);
            });
        }
    }

    [RelayCommand]
    private void ClearFilters()
    {
        SearchText = string.Empty;
        ShowInactiveOnly = false;
    }

    [RelayCommand]
    private void Refresh()
    {
        LoadSuppliersCommand.Execute(null);
    }
}

/// <summary>
/// ViewModel pour l'historique des mouvements de stock
/// </summary>
public partial class StockMovementViewModel : NavigableViewModelBase
{
    private readonly IStockService _stockService;

    [ObservableProperty]
    private ObservableCollection<StockMovement> _movements = [];

    public StockMovementViewModel(IStockService stockService)
    {
        _stockService = stockService;
    }

    public override void OnNavigatedTo(object? parameter)
    {
        LoadMovementsCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadMovementsAsync()
    {
        await ExecuteAsync(async () =>
        {
            var movements = await _stockService.GetRecentMovementsAsync(100);
            Movements = new ObservableCollection<StockMovement>(movements);
        });
    }
}

/// <summary>
/// ViewModel pour les paramètres de l'application
/// </summary>
public partial class SettingsViewModel : NavigableViewModelBase
{
    [ObservableProperty]
    private string _selectedTheme = "Light";

    [ObservableProperty]
    private bool _showLowStockAlerts = true;

    [ObservableProperty]
    private int _lowStockThreshold = 10;

    public List<string> AvailableThemes { get; } = ["Light", "Dark"];

    [RelayCommand]
    private void SaveSettings()
    {
        // TODO: Persister les paramètres
    }
}
