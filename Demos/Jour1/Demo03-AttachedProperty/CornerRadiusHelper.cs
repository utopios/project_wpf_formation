using System.Windows;
using System.Windows.Controls;

namespace AttachedPropertyDemo;

/// <summary>
/// Attached Property pour appliquer facilement des coins arrondis à n'importe quel contrôle
/// Exemple : &lt;Button local:CornerRadiusHelper.Radius="10"/&gt;
/// </summary>
public static class CornerRadiusHelper
{
    #region Radius Attached Property

    public static readonly DependencyProperty RadiusProperty =
        DependencyProperty.RegisterAttached(
            "Radius",
            typeof(double),
            typeof(CornerRadiusHelper),
            new PropertyMetadata(0.0, OnRadiusChanged));

    public static double GetRadius(DependencyObject obj)
    {
        return (double)obj.GetValue(RadiusProperty);
    }

    public static void SetRadius(DependencyObject obj, double value)
    {
        obj.SetValue(RadiusProperty, value);
    }

    private static void OnRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var radius = (double)e.NewValue;
        var cornerRadius = new CornerRadius(radius);

        // Appliquer le CornerRadius selon le type de contrôle
        switch (d)
        {
            case Border border:
                border.CornerRadius = cornerRadius;
                break;

            case Button button:
                // Pour les boutons, modifier le template si possible
                ApplyCornerRadiusToButton(button, cornerRadius);
                break;

            case TextBox textBox:
                // Enrober dans un Border avec CornerRadius
                WrapInBorder(textBox, cornerRadius);
                break;
        }
    }

    #endregion

    #region Helper Methods

    private static void ApplyCornerRadiusToButton(Button button, CornerRadius radius)
    {
        // Utiliser un style pour appliquer le CornerRadius
        var style = new Style(typeof(Button), button.Style);

        var template = new ControlTemplate(typeof(Button));
        var factory = new FrameworkElementFactory(typeof(Border));
        factory.SetValue(Border.CornerRadiusProperty, radius);
        factory.SetValue(Border.BackgroundProperty, button.Background);
        factory.SetValue(Border.BorderBrushProperty, button.BorderBrush);
        factory.SetValue(Border.BorderThicknessProperty, button.BorderThickness);

        var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
        contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
        contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
        factory.AppendChild(contentPresenter);

        template.VisualTree = factory;
        style.Setters.Add(new Setter(Control.TemplateProperty, template));

        button.Style = style;
    }

    private static void WrapInBorder(FrameworkElement element, CornerRadius radius)
    {
        if (element.Parent is not Panel panel) return;

        var border = new Border
        {
            CornerRadius = radius,
            BorderBrush = element.GetValue(Control.BorderBrushProperty) as System.Windows.Media.Brush,
            BorderThickness = element is Control control ? control.BorderThickness : new Thickness(1),
            Child = element
        };

        var index = panel.Children.IndexOf(element);
        panel.Children.RemoveAt(index);
        panel.Children.Insert(index, border);
    }

    #endregion
}
