using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MakeWish.Desktop.Abstract;

namespace MakeWish.Desktop.Dialogs;

public sealed partial class OkDialog : DialogBase
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