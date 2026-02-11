using System.Globalization;
using System.Windows.Data;

namespace AopDemo.Converters;

/// <summary>
/// Extrait le type d'aspect depuis une ligne de log pour le colorer.
/// "[12:00:00.000] [Clamp] ..." → retourne "Clamp"
/// </summary>
public class LogColorConverter : IValueConverter
{
    public static readonly LogColorConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string line)
        {
            if (line.Contains("[Log]")) return "Log";
            if (line.Contains("[Clamp]")) return "Clamp";
            if (line.Contains("[AutoDirty]")) return "AutoDirty";
            if (line.Contains("[DependsOn]")) return "DependsOn";
            if (line.Contains("[Timed]")) return "Timed";
        }

        return "Default";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
