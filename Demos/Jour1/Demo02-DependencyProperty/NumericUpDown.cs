using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;

namespace Demo02_DependencyProperty;

/// <summary>
/// Contrôle NumericUpDown démontrant les Dependency Properties
/// </summary>
public class NumericUpDown : Control
{
    #region Value Property

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(double),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(
                0.0,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                FrameworkPropertyMetadataOptions.Journal,
                OnValueChanged,
                CoerceValue),
            ValidateValue);

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (NumericUpDown)d;
        var oldValue = (double)e.OldValue;
        var newValue = (double)e.NewValue;

        Debug.WriteLine($"[OnValueChanged] {oldValue} → {newValue}");

        // Déclencher l'événement routé
        control.RaiseEvent(new RoutedPropertyChangedEventArgs<double>(
            oldValue, newValue, ValueChangedEvent));
    }

    private static object CoerceValue(DependencyObject d, object baseValue)
    {
        var control = (NumericUpDown)d;
        var value = (double)baseValue;

        Debug.WriteLine($"[CoerceValue] Input: {value}, Min: {control.Minimum}, Max: {control.Maximum}");

        // Limiter entre Min et Max
        if (value < control.Minimum)
        {
            Debug.WriteLine($"[CoerceValue] Coerced to Minimum: {control.Minimum}");
            return control.Minimum;
        }

        if (value > control.Maximum)
        {
            Debug.WriteLine($"[CoerceValue] Coerced to Maximum: {control.Maximum}");
            return control.Maximum;
        }

        return value;
    }

    private static bool ValidateValue(object value)
    {
        var d = (double)value;
        var isValid = !double.IsNaN(d) && !double.IsInfinity(d);

        Debug.WriteLine($"[ValidateValue] {value} → {(isValid ? "Valid" : "Invalid")}");

        return isValid;
    }

    #endregion

    #region Minimum Property

    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(
            nameof(Minimum),
            typeof(double),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(
                0.0,
                OnMinimumChanged,
                CoerceMinimum),
            ValidateValue);

    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (NumericUpDown)d;
        Debug.WriteLine($"[OnMinimumChanged] {e.OldValue} → {e.NewValue}");

        // Forcer la re-coercion de Maximum et Value
        control.CoerceValue(MaximumProperty);
        control.CoerceValue(ValueProperty);
    }

    private static object CoerceMinimum(DependencyObject d, object baseValue)
    {
        // Minimum ne peut pas être supérieur à Maximum
        var control = (NumericUpDown)d;
        var value = (double)baseValue;

        // Note: Maximum n'est peut-être pas encore défini lors de l'initialisation
        return value;
    }

    #endregion

    #region Maximum Property

    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(
            nameof(Maximum),
            typeof(double),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(
                100.0,
                OnMaximumChanged,
                CoerceMaximum),
            ValidateValue);

    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (NumericUpDown)d;
        Debug.WriteLine($"[OnMaximumChanged] {e.OldValue} → {e.NewValue}");

        // Forcer la re-coercion de Value
        control.CoerceValue(ValueProperty);
    }

    private static object CoerceMaximum(DependencyObject d, object baseValue)
    {
        var control = (NumericUpDown)d;
        var value = (double)baseValue;

        // Maximum ne peut pas être inférieur à Minimum
        if (value < control.Minimum)
        {
            Debug.WriteLine($"[CoerceMaximum] Coerced to Minimum: {control.Minimum}");
            return control.Minimum;
        }

        return value;
    }

    #endregion

    #region Increment Property

    public static readonly DependencyProperty IncrementProperty =
        DependencyProperty.Register(
            nameof(Increment),
            typeof(double),
            typeof(NumericUpDown),
            new PropertyMetadata(1.0));

    public double Increment
    {
        get => (double)GetValue(IncrementProperty);
        set => SetValue(IncrementProperty, value);
    }

    #endregion

    #region ValueChanged Event

    public static readonly RoutedEvent ValueChangedEvent =
        EventManager.RegisterRoutedEvent(
            nameof(ValueChanged),
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<double>),
            typeof(NumericUpDown));

    public event RoutedPropertyChangedEventHandler<double> ValueChanged
    {
        add => AddHandler(ValueChangedEvent, value);
        remove => RemoveHandler(ValueChangedEvent, value);
    }

    #endregion

    #region Commands

    public void IncrementValue()
    {
        Value += Increment;
    }

    public void DecrementValue()
    {
        Value -= Increment;
    }

    #endregion

    static NumericUpDown()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(typeof(NumericUpDown)));
    }
}
