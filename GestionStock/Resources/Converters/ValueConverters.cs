using System.Globalization;

namespace GestionStock.Resources.Converters;

/// <summary>
/// Inverse un booléen
/// </summary>
public class InverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b && !b;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b && !b;
    }
}

/// <summary>
/// Inverse booléen vers Visibility
/// </summary>
public class InverseBooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b && !b ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Null vers Visibility
/// </summary>
public class NullToVisibilityConverter : IValueConverter
{
    public bool Invert { get; set; } = false;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var isNull = value == null;
        var visible = Invert ? isNull : !isNull;
        return visible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Convertit le statut de stock en texte
/// </summary>
public class StockStatusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Product product)
        {
            if (product.IsOutOfStock) return "Rupture";
            if (product.IsLowStock) return "Stock faible";
            return "En stock";
        }
        return "Inconnu";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Convertit le statut de stock en couleur
/// </summary>
public class StockStatusColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Product product)
        {
            if (product.IsOutOfStock) return new SolidColorBrush(Color.FromRgb(244, 67, 54)); // Red
            if (product.IsLowStock) return new SolidColorBrush(Color.FromRgb(255, 152, 0)); // Orange
            return new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Green
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Convertit le type de mouvement en texte lisible
/// </summary>
public class MovementTypeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is MovementType type)
        {
            return type switch
            {
                MovementType.StockIn => "Entrée",
                MovementType.StockOut => "Sortie",
                MovementType.Adjustment => "Ajustement",
                MovementType.Return => "Retour",
                MovementType.Inventory => "Inventaire",
                _ => type.ToString()
            };
        }
        return value?.ToString() ?? "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Formate un nombre en devise
/// </summary>
public class CurrencyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal d)
        {
            return d.ToString("C2", new CultureInfo("fr-FR"));
        }
        if (value is double dbl)
        {
            return dbl.ToString("C2", new CultureInfo("fr-FR"));
        }
        return value?.ToString() ?? "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string s && decimal.TryParse(s, NumberStyles.Currency,
            new CultureInfo("fr-FR"), out var result))
        {
            return result;
        }
        return 0m;
    }
}

/// <summary>
/// Convertit un booléen en largeur (menu étendu ou réduit)
/// </summary>
public class BoolToWidthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b && b ? 240.0 : 60.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Convertit un booléen en flèche (← ou →)
/// </summary>
public class BoolToArrowConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b && b ? "←" : "→";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Convertit le type de notification en couleur
/// </summary>
public class NotificationTypeToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string type)
        {
            return type switch
            {
                "Success" => new SolidColorBrush(Color.FromRgb(76, 175, 80)), // Green
                "Error" => new SolidColorBrush(Color.FromRgb(244, 67, 54)), // Red
                "Warning" => new SolidColorBrush(Color.FromRgb(255, 152, 0)), // Orange
                "Info" => new SolidColorBrush(Color.FromRgb(33, 150, 243)), // Blue
                _ => new SolidColorBrush(Color.FromRgb(158, 158, 158)) // Gray
            };
        }
        return new SolidColorBrush(Color.FromRgb(158, 158, 158));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
