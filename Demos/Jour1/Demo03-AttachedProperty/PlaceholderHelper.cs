using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AttachedPropertyDemo;

/// <summary>
/// Attached Property pour ajouter un placeholder (texte d'aide) aux TextBox
/// Exemple : &lt;TextBox local:PlaceholderHelper.Text="Entrez votre nom..."/&gt;
/// </summary>
public static class PlaceholderHelper
{
    #region Placeholder Text Attached Property

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.RegisterAttached(
            "Text",
            typeof(string),
            typeof(PlaceholderHelper),
            new PropertyMetadata(string.Empty, OnPlaceholderTextChanged));

    public static string GetText(DependencyObject obj)
    {
        return (string)obj.GetValue(TextProperty);
    }

    public static void SetText(DependencyObject obj, string value)
    {
        obj.SetValue(TextProperty, value);
    }

    private static void OnPlaceholderTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBox textBox) return;

        textBox.GotFocus -= RemovePlaceholder;
        textBox.LostFocus -= ShowPlaceholder;
        textBox.TextChanged -= OnTextChanged;

        if (!string.IsNullOrEmpty(e.NewValue as string))
        {
            textBox.GotFocus += RemovePlaceholder;
            textBox.LostFocus += ShowPlaceholder;
            textBox.TextChanged += OnTextChanged;

            ShowPlaceholder(textBox, null!);
        }
    }

    #endregion

    #region Placeholder Color Attached Property

    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.RegisterAttached(
            "Color",
            typeof(Brush),
            typeof(PlaceholderHelper),
            new PropertyMetadata(Brushes.Gray));

    public static Brush GetColor(DependencyObject obj)
    {
        return (Brush)obj.GetValue(ColorProperty);
    }

    public static void SetColor(DependencyObject obj, Brush value)
    {
        obj.SetValue(ColorProperty, value);
    }

    #endregion

    #region Helper Methods

    private static void ShowPlaceholder(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        if (string.IsNullOrEmpty(textBox.Text) && !textBox.IsFocused)
        {
            textBox.Foreground = GetColor(textBox);
            textBox.Text = GetText(textBox);
            textBox.Tag = "placeholder";
        }
    }

    private static void RemovePlaceholder(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        if (textBox.Tag?.ToString() == "placeholder")
        {
            textBox.Text = string.Empty;
            textBox.Foreground = Brushes.Black;
            textBox.Tag = null;
        }
    }

    private static void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender;
        if (!string.IsNullOrEmpty(textBox.Text) && textBox.Tag?.ToString() == "placeholder")
        {
            textBox.Foreground = Brushes.Black;
            textBox.Tag = null;
        }
    }

    #endregion
}
