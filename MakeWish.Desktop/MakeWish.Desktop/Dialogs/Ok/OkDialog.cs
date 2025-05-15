using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MakeWish.Desktop.Abstract;

namespace MakeWish.Desktop.Dialogs.Ok;

public sealed partial class OkDialog : Dialog
{
    public event Action? OnOkCommand;
    
    [ObservableProperty]
    private string _message = string.Empty;

    public OkDialog(string message)
    {
        Message = message;
    }

    [RelayCommand]
    private void Ok()
    {
        OnOkCommand?.Invoke();
    }
}