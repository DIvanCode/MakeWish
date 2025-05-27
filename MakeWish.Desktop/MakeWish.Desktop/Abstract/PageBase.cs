using CommunityToolkit.Mvvm.ComponentModel;
using FluentResults;
using MakeWish.Desktop.Clients.Common.UserContext;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Abstract;

public abstract class PageBase : ObservableObject
{
    public abstract Task<Result> LoadDataAsync(CancellationToken cancellationToken);
}