using System.Windows;
using System.Windows.Controls;

namespace Demo02_DependencyProperty;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void NumericControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        LogEvent($"ValueChanged: {e.OldValue:F2} → {e.NewValue:F2}");
    }

    private void Increment_Click(object sender, RoutedEventArgs e)
    {
        NumericControl.IncrementValue();
    }

    private void Decrement_Click(object sender, RoutedEventArgs e)
    {
        NumericControl.DecrementValue();
    }

    private void MinMax_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Vérifier que tous les contrôles sont initialisés
        if (NumericControl == null || MinTextBox == null || MaxTextBox == null) return;

        if (double.TryParse(MinTextBox.Text, out var min))
        {
            NumericControl.Minimum = min;
            LogEvent($"Minimum changed to: {min}");
        }

        if (double.TryParse(MaxTextBox.Text, out var max))
        {
            NumericControl.Maximum = max;
            LogEvent($"Maximum changed to: {max}");
        }
    }

    private void Increment_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Vérifier que tous les contrôles sont initialisés
        if (NumericControl == null || IncrementTextBox == null) return;

        if (double.TryParse(IncrementTextBox.Text, out var increment))
        {
            NumericControl.Increment = increment;
        }
    }

    private void SetValue_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(SetValueTextBox.Text, out var value))
        {
            LogEvent($"Setting value to: {value}");
            NumericControl.Value = value;
        }
    }

    private void TestInvalid_Click(object sender, RoutedEventArgs e)
    {
        LogEvent("Attempting to set NaN (should fail validation)...");
        try
        {
            NumericControl.Value = double.NaN;
        }
        catch (Exception ex)
        {
            LogEvent($"Exception: {ex.Message}");
        }
    }

    private void TestOutOfRange_Click(object sender, RoutedEventArgs e)
    {
        LogEvent($"Setting value to 500 (Max is {NumericControl.Maximum})...");
        NumericControl.Value = 500;
        LogEvent($"Result: Value is {NumericControl.Value} (coerced to Max)");
    }

    private void LogEvent(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        EventLog?.Items.Insert(0, $"[{timestamp}] {message}");

        // Limiter le nombre d'entrées
        while (EventLog?.Items.Count > 50)
        {
            EventLog.Items.RemoveAt(EventLog.Items.Count - 1);
        }
    }
}
