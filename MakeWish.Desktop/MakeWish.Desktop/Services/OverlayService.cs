using FluentResults;
using MakeWish.Desktop.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace MakeWish.Desktop.Services;

internal interface IOverlayService
{
    event Action<OverlayBase?>? OnChangeOverlay;

    OverlayBase? Current { get; }

    void Show<TOverlay>(params object[] p) where TOverlay : OverlayBase;
    
    void Close();
    void Clear();
}

internal sealed class OverlayService(IServiceProvider serviceProvider, IAsyncExecutor asyncExecutor) : IOverlayService
{
    private readonly List<OverlayBase> _overlays = [];
    private Action? _showAction;

    public event Action<OverlayBase?>? OnChangeOverlay;

    public OverlayBase? Current => _overlays.LastOrDefault();

    public void Show<TOverlay>(params object[] p) where TOverlay : OverlayBase
    {
        var overlay = ActivatorUtilities.CreateInstance<TOverlay>(serviceProvider, p);
        asyncExecutor.Execute(async cancellationToken =>
        {
            var result = await overlay.LoadDataAsync(cancellationToken);
            if (result.IsFailed)
            {
                return result;
            }
            
            Open(overlay);
            return Result.Ok();
        });
    }

    public void ConfirmShow()
    {
        _showAction?.Invoke();
        _showAction = null;
    }

    public void Close()
    {
        if (_overlays.Count > 0)
        {
            _overlays.RemoveAt(_overlays.Count - 1);
        }
        
        OnChangeOverlay?.Invoke(_overlays.LastOrDefault());
    }

    public void Clear()
    {
        if (_overlays.Count == 0)
        {
            return;
        }
        
        _overlays.Clear();
        OnChangeOverlay?.Invoke(null);
    }

    private void Open(OverlayBase overlay)
    {
        _overlays.Add(overlay);
        OnChangeOverlay?.Invoke(overlay);
    }
}