namespace GestionStock.ViewModels;

/// <summary>
/// ViewModel pour l'édition/création d'un fournisseur
/// </summary>
public partial class SupplierEditViewModel : NavigableViewModelBase
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;

    private int? _supplierId;
    private bool _isNew;

    [ObservableProperty]
    private string _title = "Nouveau fournisseur";

    // Propriétés du formulaire avec validation
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Le nom du fournisseur est requis")]
    [MinLength(2, ErrorMessage = "Le nom doit contenir au moins 2 caractères")]
    [MaxLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _name = string.Empty;

    [ObservableProperty]
    [MaxLength(100, ErrorMessage = "Le nom du contact ne peut pas dépasser 100 caractères")]
    private string? _contactName;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [EmailAddress(ErrorMessage = "Email invalide")]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string? _email;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Phone(ErrorMessage = "Numéro de téléphone invalide")]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string? _phone;

    [ObservableProperty]
    [MaxLength(200, ErrorMessage = "L'adresse ne peut pas dépasser 200 caractères")]
    private string? _address;

    [ObservableProperty]
    private bool _isActive = true;

    public SupplierEditViewModel(
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
        if (parameter is int supplierId)
        {
            _supplierId = supplierId;
            _isNew = false;
            Title = "Modifier le fournisseur";
        }
        else
        {
            _isNew = true;
            Title = "Nouveau fournisseur";
        }

        LoadDataCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        if (!_supplierId.HasValue) return;

        await ExecuteAsync(async () =>
        {
            var supplier = await _supplierRepository.GetByIdAsync(_supplierId.Value);
            if (supplier != null)
            {
                Name = supplier.Name;
                ContactName = supplier.ContactName;
                Email = supplier.Email;
                Phone = supplier.Phone;
                Address = supplier.Address;
                IsActive = supplier.IsActive;
            }
        });
    }

    private bool CanSave()
    {
        ValidateAllProperties();
        return !HasErrors && !string.IsNullOrWhiteSpace(Name);
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        ValidateAllProperties();
        if (HasErrors) return;

        await ExecuteAsync(async () =>
        {
            if (_isNew)
            {
                var supplier = new Supplier
                {
                    Name = Name,
                    ContactName = ContactName,
                    Email = Email,
                    Phone = Phone,
                    Address = Address,
                    IsActive = IsActive
                };

                await _supplierRepository.AddAsync(supplier);
                _dialogService.ShowNotification("Fournisseur créé avec succès", NotificationType.Success);
            }
            else
            {
                var supplier = await _supplierRepository.GetByIdAsync(_supplierId!.Value)
                    ?? throw new InvalidOperationException("Fournisseur non trouvé");

                supplier.Name = Name;
                supplier.ContactName = ContactName;
                supplier.Email = Email;
                supplier.Phone = Phone;
                supplier.Address = Address;
                supplier.IsActive = IsActive;

                await _supplierRepository.UpdateAsync(supplier);
                _dialogService.ShowNotification("Fournisseur modifié avec succès", NotificationType.Success);
            }

            _navigationService.GoBack();
        });
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        if (HasChanges())
        {
            var confirm = await _dialogService.ConfirmAsync(
                "Modifications non sauvegardées",
                "Voulez-vous vraiment annuler ? Les modifications seront perdues.");

            if (!confirm) return;
        }

        _navigationService.GoBack();
    }

    private bool HasChanges()
    {
        return !string.IsNullOrEmpty(Name) || !string.IsNullOrEmpty(ContactName) ||
               !string.IsNullOrEmpty(Email) || !string.IsNullOrEmpty(Phone);
    }
}
