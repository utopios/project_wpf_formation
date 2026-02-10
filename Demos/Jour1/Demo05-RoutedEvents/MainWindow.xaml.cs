using System.Windows;
using System.Windows.Input;

namespace RoutedEventsDemo;

public partial class MainWindow : Window
{
    private int _eventCounter = 0;

    public MainWindow()
    {
        InitializeComponent();
    }

    #region Tunneling Events (Preview*)

    private void OuterBorder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        LogEvent("TUNNEL", "OuterBorder", "PreviewMouseDown", "#9C27B0");
    }

    private void MiddleBorder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        LogEvent("TUNNEL", "MiddleBorder", "PreviewMouseDown", "#9C27B0");

        // Optionnel : arrêter la propagation
        if (StopTunnelingCheckBox.IsChecked == true)
        {
            e.Handled = true;
            LogEvent("STOP", "MiddleBorder", "Tunneling arrêté (e.Handled = true)", "#E91E63");
        }
    }

    private void InnerBorder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        LogEvent("TUNNEL", "InnerBorder", "PreviewMouseDown", "#9C27B0");
    }

    private void ActionButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        LogEvent("TUNNEL", "Button", "PreviewMouseDown", "#9C27B0");
    }

    #endregion

    #region Bubbling Events

    private void OuterBorder_MouseDown(object sender, MouseButtonEventArgs e)
    {
        LogEvent("BUBBLE", "OuterBorder", "MouseDown", "#2196F3");
    }

    private void MiddleBorder_MouseDown(object sender, MouseButtonEventArgs e)
    {
        LogEvent("BUBBLE", "MiddleBorder", "MouseDown", "#2196F3");

        // Optionnel : arrêter la propagation
        if (StopBubblingCheckBox.IsChecked == true)
        {
            e.Handled = true;
            LogEvent("STOP", "MiddleBorder", "Bubbling arrêté (e.Handled = true)", "#E91E63");
        }
    }

    private void InnerBorder_MouseDown(object sender, MouseButtonEventArgs e)
    {
        LogEvent("BUBBLE", "InnerBorder", "MouseDown", "#2196F3");
    }

    private void ActionButton_Click(object sender, RoutedEventArgs e)
    {
        // Click est un événement Bubbling
        LogEvent("BUBBLE", "Button", "Click", "#4CAF50");
    }

    #endregion

    #region Helpers

    private void LogEvent(string type, string source, string eventName, string color)
    {
        _eventCounter++;
        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        var message = $"[{_eventCounter:D3}] {type}: {source}.{eventName}";

        // Ajouter en haut de la liste
        EventLog.Items.Insert(0, message);

        // Limiter à 100 entrées
        while (EventLog.Items.Count > 100)
        {
            EventLog.Items.RemoveAt(EventLog.Items.Count - 1);
        }
    }

    private void ClearLog_Click(object sender, RoutedEventArgs e)
    {
        EventLog.Items.Clear();
        _eventCounter = 0;
    }

    #endregion
}
