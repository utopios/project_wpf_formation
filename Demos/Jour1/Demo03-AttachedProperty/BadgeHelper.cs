using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AttachedPropertyDemo;

/// <summary>
/// Attached Property pour ajouter un badge (notification) sur n'importe quel contrôle
/// Exemple d'usage : &lt;Button local:BadgeHelper.Badge="3" Content="Messages"/&gt;
/// </summary>
public static class BadgeHelper
{
    #region Badge Attached Property

    public static readonly DependencyProperty BadgeProperty =
        DependencyProperty.RegisterAttached(
            "Badge",
            typeof(string),
            typeof(BadgeHelper),
            new PropertyMetadata(null, OnBadgeChanged));

    public static string? GetBadge(DependencyObject obj)
    {
        return (string?)obj.GetValue(BadgeProperty);
    }

    public static void SetBadge(DependencyObject obj, string? value)
    {
        obj.SetValue(BadgeProperty, value);
    }

    private static void OnBadgeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not FrameworkElement element) return;

        var badge = e.NewValue as string;

        if (string.IsNullOrEmpty(badge))
        {
            RemoveBadge(element);
        }
        else
        {
            AddBadge(element, badge);
        }
    }

    #endregion

    #region Helper Methods

    private static void AddBadge(FrameworkElement element, string text)
    {
        // Attendre que l'élément soit chargé
        if (!element.IsLoaded)
        {
            element.Loaded += (s, e) => AddBadge(element, text);
            return;
        }

        RemoveBadge(element);

        // Créer le badge visuel
        var badge = new Border
        {
            Background = Brushes.Red,
            CornerRadius = new CornerRadius(10),
            MinWidth = 20,
            Height = 20,
            Padding = new Thickness(5, 0, 5, 0),
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(0, -10, -10, 0),
            Child = new TextBlock
            {
                Text = text,
                Foreground = Brushes.White,
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };

        // Enrober l'élément dans un Grid si ce n'est pas déjà fait
        if (element.Parent is Panel panel)
        {
            var grid = new Grid();
            var index = panel.Children.IndexOf(element);

            panel.Children.RemoveAt(index);
            grid.Children.Add(element);
            grid.Children.Add(badge);
            panel.Children.Insert(index, grid);

            // Stocker la référence pour pouvoir la retirer plus tard
            element.SetValue(BadgeContainerProperty, grid);
        }
    }

    private static void RemoveBadge(FrameworkElement element)
    {
        if (element.GetValue(BadgeContainerProperty) is Grid grid && grid.Parent is Panel panel)
        {
            var index = panel.Children.IndexOf(grid);
            panel.Children.RemoveAt(index);
            grid.Children.Remove(element);
            panel.Children.Insert(index, element);
            element.ClearValue(BadgeContainerProperty);
        }
    }

    // Propriété privée pour stocker le conteneur du badge
    private static readonly DependencyProperty BadgeContainerProperty =
        DependencyProperty.RegisterAttached("BadgeContainer", typeof(Grid), typeof(BadgeHelper));

    #endregion
}
