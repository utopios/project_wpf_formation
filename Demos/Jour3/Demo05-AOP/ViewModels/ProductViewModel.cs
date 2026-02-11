using AopDemo.Attributes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AopDemo.ViewModels;

/// <summary>
/// ViewModel 100% propre — AUCUN code AOP.
///
/// Pas de base class AOP, pas de câblage, pas d'event subscription.
/// Juste ObservableObject + attributs déclaratifs.
/// L'interception est faite par le conteneur DI (comme Spring).
/// </summary>
public partial class ProductViewModel : ObservableObject
{
    // ── Propriétés avec aspects déclaratifs ──

    [ObservableProperty]
    [LogOnChange]
    [AutoDirty]
    [DependsOn(nameof(FullDescription))]
    private string _name = string.Empty;

    [ObservableProperty]
    [LogOnChange]
    [AutoDirty]
    [DependsOn(nameof(FullDescription))]
    private string _category = string.Empty;

    [ObservableProperty]
    [LogOnChange]
    [AutoDirty]
    [Clamp(0, 10000)]
    [DependsOn(nameof(TotalPrice), nameof(FullDescription))]
    private double _price;

    [ObservableProperty]
    [LogOnChange]
    [AutoDirty]
    [Clamp(0, 9999)]
    [DependsOn(nameof(TotalPrice), nameof(FullDescription))]
    private int _quantity;

    [ObservableProperty]
    [LogOnChange]
    [Clamp(0, 100)]
    [DependsOn(nameof(TotalPrice))]
    private double _discount;

    // ── Propriétés calculées ──

    public double TotalPrice => Price * Quantity * (1 - Discount / 100);

    public string FullDescription =>
        string.IsNullOrWhiteSpace(Name)
            ? "(aucun produit)"
            : $"{Name} ({Category}) — {Price:C2} x {Quantity} = {TotalPrice:C2}";

    // ── Commandes (logique métier pure, aucun aspect) ──

    [RelayCommand]
    private async Task LoadData()
    {
        await Task.Delay(1500); // Simulation chargement BDD
        Name = "Clavier mécanique";
        Category = "Informatique";
        Price = 89.99;
        Quantity = 25;
        Discount = 10;
    }

    [RelayCommand]
    private async Task Save()
    {
        await Task.Delay(800); // Simulation sauvegarde
    }

    [RelayCommand]
    private void TestClamp()
    {
        Price = -50;       // → clampé à 0
        Price = 99999;     // → clampé à 10000
        Quantity = -10;    // → clampé à 0
        Discount = 200;    // → clampé à 100
    }

    [RelayCommand]
    private void Reset()
    {
        Name = string.Empty;
        Category = string.Empty;
        Price = 0;
        Quantity = 0;
        Discount = 0;
    }
}
