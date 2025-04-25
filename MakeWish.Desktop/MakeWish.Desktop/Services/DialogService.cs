using System.Windows;

namespace MakeWish.Desktop.Services;

public class DialogService : IDialogService
{
    public void ShowError(string message)
    {
        MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
} 