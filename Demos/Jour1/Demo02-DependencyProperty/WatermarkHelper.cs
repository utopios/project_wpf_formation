using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Demo02_DependencyProperty;

/// <summary>
/// Démontre une Attached Property pour ajouter un watermark aux TextBox
/// </summary>
public static class WatermarkHelper
{
    #region Watermark Attached Property

    // Déclaration de l'Attached Property
    public static readonly DependencyProperty WatermarkProperty =
        DependencyProperty.RegisterAttached(
            "Watermark",
            typeof(string),
            typeof(WatermarkHelper),
            new FrameworkPropertyMetadata(string.Empty, OnWatermarkChanged));

    // Getter statique (requis pour les Attached Properties)
    public static string GetWatermark(DependencyObject obj)
    {
        return (string)obj.GetValue(WatermarkProperty);
    }

    // Setter statique (requis pour les Attached Properties)
    public static void SetWatermark(DependencyObject obj, string value)
    {
        obj.SetValue(WatermarkProperty, value);
    }

    // Callback appelé quand la propriété change
    private static void OnWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextBox textBox)
        {
            textBox.Loaded -= TextBox_Loaded;
            textBox.GotFocus -= TextBox_GotFocus;
            textBox.LostFocus -= TextBox_LostFocus;
            textBox.TextChanged -= TextBox_TextChanged;

            if (!string.IsNullOrEmpty((string)e.NewValue))
            {
                textBox.Loaded += TextBox_Loaded;
                textBox.GotFocus += TextBox_GotFocus;
                textBox.LostFocus += TextBox_LostFocus;
                textBox.TextChanged += TextBox_TextChanged;

                if (textBox.IsLoaded)
                {
                    ShowWatermark(textBox);
                }
            }
        }
    }

    #endregion

    #region Event Handlers

    private static void TextBox_Loaded(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        ShowWatermark(textBox);
    }

    private static void TextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        RemoveWatermark(textBox);
    }

    private static void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        ShowWatermark(textBox);
    }

    private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender;
        if (string.IsNullOrEmpty(textBox.Text))
        {
            ShowWatermark(textBox);
        }
        else
        {
            RemoveWatermark(textBox);
        }
    }

    #endregion

    #region Helper Methods

    private static void ShowWatermark(TextBox textBox)
    {
        if (!string.IsNullOrEmpty(textBox.Text)) return;

        var watermark = GetWatermark(textBox);
        if (string.IsNullOrEmpty(watermark)) return;

        // Utiliser un adorner pour afficher le watermark
        var adornerLayer = AdornerLayer.GetAdornerLayer(textBox);
        if (adornerLayer != null)
        {
            RemoveWatermark(textBox);
            adornerLayer.Add(new WatermarkAdorner(textBox, watermark));
        }
    }

    private static void RemoveWatermark(TextBox textBox)
    {
        var adornerLayer = AdornerLayer.GetAdornerLayer(textBox);
        if (adornerLayer != null)
        {
            var adorners = adornerLayer.GetAdorners(textBox);
            if (adorners != null)
            {
                foreach (var adorner in adorners)
                {
                    if (adorner is WatermarkAdorner)
                    {
                        adornerLayer.Remove(adorner);
                    }
                }
            }
        }
    }

    #endregion

    #region WatermarkAdorner

    private class WatermarkAdorner : Adorner
    {
        private readonly string _watermark;

        public WatermarkAdorner(UIElement adornedElement, string watermark) : base(adornedElement)
        {
            _watermark = watermark;
            IsHitTestVisible = false;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var textBox = (TextBox)AdornedElement;

            var formattedText = new FormattedText(
                _watermark,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(textBox.FontFamily, textBox.FontStyle, textBox.FontWeight, textBox.FontStretch),
                textBox.FontSize,
                Brushes.Gray,
                1.0);

            drawingContext.DrawText(formattedText, new Point(5, 5));
        }
    }

    #endregion
}
