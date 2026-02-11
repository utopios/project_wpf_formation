# Démo - Tests Unitaires MVVM

Cette démo montre comment tester les ViewModels en WPF/MVVM.

## Exécuter les tests

```bash
dotnet test
```

## Concepts démontrés

### 1. Pattern AAA (Arrange-Act-Assert)

```csharp
[Fact]
public void Test_Example()
{
    // Arrange - Préparer le contexte
    var viewModel = new MyViewModel();

    // Act - Exécuter l'action
    viewModel.DoSomething();

    // Assert - Vérifier le résultat
    viewModel.Property.Should().Be(expected);
}
```

### 2. Mocking avec Moq

```csharp
var mock = new Mock<IService>();
mock.Setup(s => s.GetData()).Returns("test");

var vm = new ViewModel(mock.Object);
// ...

mock.Verify(s => s.GetData(), Times.Once);
```

### 3. Tests Theory avec paramètres

```csharp
[Theory]
[InlineData(0, "Épuisé")]
[InlineData(5, "Stock faible")]
public void StockStatus_Test(int stock, string expected)
{
    // Test paramétré
}
```

### 4. Tests des notifications INPC

```csharp
var raised = false;
vm.PropertyChanged += (_, e) => {
    if (e.PropertyName == "Name") raised = true;
};

vm.Name = "test";

raised.Should().BeTrue();
```

## Frameworks utilisés

- **xUnit** : Framework de test
- **Moq** : Mocking
- **FluentAssertions** : Assertions lisibles
