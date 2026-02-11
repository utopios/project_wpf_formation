using FluentAssertions;
using Moq;
using TestsDemo.ViewModels;
using Xunit;

namespace TestsDemo.Tests;

/// <summary>
/// Tests unitaires pour ProductViewModel
/// Démontre les bonnes pratiques de test MVVM
/// </summary>
public class ProductViewModelTests
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly ProductViewModel _viewModel;

    /// <summary>
    /// Setup commun pour tous les tests
    /// </summary>
    public ProductViewModelTests()
    {
        _mockProductService = new Mock<IProductService>();
        _viewModel = new ProductViewModel(_mockProductService.Object);
    }

    #region Tests des propriétés calculées

    [Fact]
    public void IsValid_WhenAllFieldsValid_ReturnsTrue()
    {
        // Arrange
        _viewModel.Name = "Produit Test";
        _viewModel.Price = 99.99m;
        _viewModel.Stock = 10;

        // Act
        var result = _viewModel.IsValid;

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("", 10, 5)]      // Nom vide
    [InlineData("Test", 0, 5)]   // Prix à 0
    [InlineData("Test", -1, 5)]  // Prix négatif
    [InlineData("Test", 10, -1)] // Stock négatif
    public void IsValid_WhenFieldInvalid_ReturnsFalse(string name, decimal price, int stock)
    {
        // Arrange
        _viewModel.Name = name;
        _viewModel.Price = price;
        _viewModel.Stock = stock;

        // Act
        var result = _viewModel.IsValid;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void FormattedPrice_ReturnsCorrectFormat()
    {
        // Arrange
        _viewModel.Price = 123.45m;

        // Act
        var result = _viewModel.FormattedPrice;

        // Assert
        result.Should().Contain("123");
        result.Should().Contain("45");
    }

    [Theory]
    [InlineData(0, "Épuisé")]
    [InlineData(3, "Stock faible")]
    [InlineData(5, "Stock faible")]
    [InlineData(10, "Stock normal")]
    [InlineData(20, "Stock normal")]
    [InlineData(50, "Stock élevé")]
    public void StockStatus_ReturnsCorrectStatus(int stock, string expectedStatus)
    {
        // Arrange
        _viewModel.Stock = stock;

        // Act
        var result = _viewModel.StockStatus;

        // Assert
        result.Should().Be(expectedStatus);
    }

    #endregion

    #region Tests des commandes

    [Fact]
    public void ResetCommand_ClearsAllFields()
    {
        // Arrange
        _viewModel.Name = "Test";
        _viewModel.Price = 100;
        _viewModel.Stock = 50;

        // Act
        _viewModel.ResetCommand.Execute(null);

        // Assert
        _viewModel.Name.Should().BeEmpty();
        _viewModel.Price.Should().Be(0);
        _viewModel.Stock.Should().Be(0);
    }

    [Fact]
    public void IncrementStockCommand_IncreasesStock()
    {
        // Arrange
        _viewModel.Stock = 5;

        // Act
        _viewModel.IncrementStockCommand.Execute(null);

        // Assert
        _viewModel.Stock.Should().Be(6);
    }

    [Fact]
    public void DecrementStockCommand_DecreasesStock()
    {
        // Arrange
        _viewModel.Stock = 5;

        // Act
        _viewModel.DecrementStockCommand.Execute(null);

        // Assert
        _viewModel.Stock.Should().Be(4);
    }

    [Fact]
    public void DecrementStockCommand_CannotExecute_WhenStockIsZero()
    {
        // Arrange
        _viewModel.Stock = 0;

        // Act
        var canExecute = _viewModel.DecrementStockCommand.CanExecute(null);

        // Assert
        canExecute.Should().BeFalse();
    }

    [Fact]
    public void SaveCommand_CannotExecute_WhenInvalid()
    {
        // Arrange - données invalides
        _viewModel.Name = "";
        _viewModel.Price = 0;

        // Act
        var canExecute = _viewModel.SaveCommand.CanExecute(null);

        // Assert
        canExecute.Should().BeFalse();
    }

    [Fact]
    public void SaveCommand_CanExecute_WhenValid()
    {
        // Arrange
        _viewModel.Name = "Produit";
        _viewModel.Price = 50;
        _viewModel.Stock = 10;

        // Act
        var canExecute = _viewModel.SaveCommand.CanExecute(null);

        // Assert
        canExecute.Should().BeTrue();
    }

    #endregion

    #region Tests avec Mocks

    [Fact]
    public async Task SaveCommand_CallsProductService()
    {
        // Arrange
        _viewModel.Name = "Nouveau Produit";
        _viewModel.Price = 49.99m;
        _viewModel.Stock = 100;

        _mockProductService
            .Setup(s => s.SaveProductAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        // Act
        await _viewModel.SaveCommand.ExecuteAsync(null);

        // Assert - Vérifie que le service a été appelé
        _mockProductService.Verify(
            s => s.SaveProductAsync(It.Is<Product>(p =>
                p.Name == "Nouveau Produit" &&
                p.Price == 49.99m &&
                p.Stock == 100)),
            Times.Once);
    }

    [Fact]
    public async Task SaveCommand_SetsErrorMessage_OnException()
    {
        // Arrange
        _viewModel.Name = "Produit";
        _viewModel.Price = 50;
        _viewModel.Stock = 10;

        _mockProductService
            .Setup(s => s.SaveProductAsync(It.IsAny<Product>()))
            .ThrowsAsync(new Exception("Erreur de sauvegarde"));

        // Act
        await _viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        _viewModel.ErrorMessage.Should().Be("Erreur de sauvegarde");
    }

    [Fact]
    public async Task SaveCommand_SetsIsLoading_DuringExecution()
    {
        // Arrange
        _viewModel.Name = "Produit";
        _viewModel.Price = 50;
        _viewModel.Stock = 10;

        var isLoadingDuringExecution = false;

        _mockProductService
            .Setup(s => s.SaveProductAsync(It.IsAny<Product>()))
            .Returns(async () =>
            {
                isLoadingDuringExecution = _viewModel.IsLoading;
                await Task.Delay(10);
            });

        // Act
        await _viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        isLoadingDuringExecution.Should().BeTrue();
        _viewModel.IsLoading.Should().BeFalse(); // Après exécution
    }

    #endregion

    #region Tests des notifications PropertyChanged

    [Fact]
    public void SettingName_RaisesPropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        _viewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ProductViewModel.Name))
                propertyChangedRaised = true;
        };

        // Act
        _viewModel.Name = "Nouveau nom";

        // Assert
        propertyChangedRaised.Should().BeTrue();
    }

    [Fact]
    public void SettingPrice_NotifiesFormattedPrice()
    {
        // Arrange
        var notifiedProperties = new List<string>();
        _viewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName != null)
                notifiedProperties.Add(e.PropertyName);
        };

        // Act
        _viewModel.Price = 100;

        // Assert
        notifiedProperties.Should().Contain(nameof(ProductViewModel.FormattedPrice));
    }

    [Fact]
    public void SettingStock_NotifiesStockStatus()
    {
        // Arrange
        var notifiedProperties = new List<string>();
        _viewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName != null)
                notifiedProperties.Add(e.PropertyName);
        };

        // Act
        _viewModel.Stock = 5;

        // Assert
        notifiedProperties.Should().Contain(nameof(ProductViewModel.StockStatus));
    }

    #endregion
}
