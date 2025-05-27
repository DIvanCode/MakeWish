using CommunityToolkit.Mvvm.ComponentModel;
using FluentResults;

namespace MakeWish.Desktop.Abstract;

public abstract class OverlayBase : ObservableObject
{
    public abstract Task<Result> LoadDataAsync(CancellationToken cancellationToken);
}