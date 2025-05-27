using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MakeWish.Desktop.Abstract;

namespace MakeWish.Desktop.Dialogs;

public sealed partial class YesNoDialog : DialogBase
{
    public event Action? OnYesCommand;
    public event Action? OnNoCommand;
    
    [ObservableProperty]
    private string _message = string.Empty;

    public YesNoDialog(string message)
    {
        Message = message;
    }

    [RelayCommand]
    private void Yes()
    {
        OnYesCommand?.Invoke();
    }
    
    [RelayCommand]
    private void No()
    {
        OnNoCommand?.Invoke();
    }
}