using FluentResults;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Dialogs;

namespace MakeWish.Desktop.Services;

internal interface IDialogService
{
    event Action<DialogBase?>? OnChangeDialog;

    DialogBase? Current { get; }
    
    void ShowMessage(string message);
    void ShowErrors(List<IError> error);
    void ShowYesNoDialog(string message, Action onYesCommand, Action? onNoCommand = null);

    void Close();
}

internal sealed class DialogService : IDialogService
{
    public event Action<DialogBase?>? OnChangeDialog;
    
    public DialogBase? Current { get; private set; }
    
    public void ShowMessage(string message)
    {
        var dialog = new OkDialog(message);
        dialog.OnOkCommand += Close;
        
        OnChangeDialog?.Invoke(dialog);
    }

    public void ShowErrors(List<IError> errors)
    {
        ShowMessage(string.Join("\n", errors.Select(error => error.Message)));
    }

    public void ShowYesNoDialog(string message, Action onYesCommand, Action? onNoCommand = null)
    {
        var dialog = new YesNoDialog(message);
        dialog.OnYesCommand += onYesCommand;
        dialog.OnYesCommand += Close;
        dialog.OnNoCommand += onNoCommand ?? Close;
        
        OnChangeDialog?.Invoke(dialog);
    }
    
    public void Close()
    {
        OnChangeDialog?.Invoke(null);
    }
}