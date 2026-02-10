namespace GestionStock.Services;

/// <summary>
/// Service de dialogue et notifications
/// </summary>
public class DialogService : IDialogService
{
    public Task<bool> ConfirmAsync(string title, string message)
    {
        var result = MessageBox.Show(
            message,
            title,
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        return Task.FromResult(result == MessageBoxResult.Yes);
    }

    public Task AlertAsync(string title, string message)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        return Task.CompletedTask;
    }

    public Task<string?> PromptAsync(string title, string message, string defaultValue = "")
    {
        // Dans une vraie application, créer une fenêtre de dialogue personnalisée
        // Pour simplifier, on utilise une InputBox basique

        var dialog = new Window
        {
            Title = title,
            Width = 400,
            Height = 180,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = Application.Current.MainWindow,
            ResizeMode = ResizeMode.NoResize
        };

        var panel = new StackPanel { Margin = new Thickness(20) };

        var label = new TextBlock { Text = message, Margin = new Thickness(0, 0, 0, 10) };
        var textBox = new TextBox { Text = defaultValue };

        var buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 20, 0, 0)
        };

        var okButton = new Button { Content = "OK", Width = 80, IsDefault = true };
        var cancelButton = new Button { Content = "Annuler", Width = 80, Margin = new Thickness(10, 0, 0, 0), IsCancel = true };

        string? result = null;

        okButton.Click += (s, e) => { result = textBox.Text; dialog.DialogResult = true; };
        cancelButton.Click += (s, e) => { dialog.DialogResult = false; };

        buttonPanel.Children.Add(okButton);
        buttonPanel.Children.Add(cancelButton);

        panel.Children.Add(label);
        panel.Children.Add(textBox);
        panel.Children.Add(buttonPanel);

        dialog.Content = panel;

        textBox.Focus();
        textBox.SelectAll();

        dialog.ShowDialog();

        return Task.FromResult(result);
    }

    public void ShowNotification(string message, NotificationType type = NotificationType.Info)
    {
        // Envoyer un message pour afficher la notification dans l'UI
        WeakReferenceMessenger.Default.Send(new NotificationMessage(message, type));
    }
}

/// <summary>
/// Message de notification pour l'UI
/// </summary>
public record NotificationMessage(string Message, NotificationType Type);
