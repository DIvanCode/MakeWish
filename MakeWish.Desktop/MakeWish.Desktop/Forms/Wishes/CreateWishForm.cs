using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Clients.WishService.Requests.Wishes;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Pages.Wishes;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Forms.Wishes;

internal sealed partial class CreateWishForm(
    INavigationService navigationService,
    IOverlayService overlayService,
    IAsyncExecutor asyncExecutor,
    IWishServiceClient wishServiceClient)
    : OverlayBase
{
    [ObservableProperty]
    private string _title = string.Empty;
    
    [ObservableProperty]
    private string _description = string.Empty;
    
    [ObservableProperty]
    private bool _isPublic;
    
    public override Task<Result> LoadDataAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(Result.Ok());
    }

    [RelayCommand]
    private void Close()
    {
        overlayService.Close();
    }

    [RelayCommand]
    private void Create()
    {
        asyncExecutor.Execute(async cancellationToken =>
        {
            var request = new CreateWishRequest
            {
                Title = Title,
                Description = Description,
                IsPublic = IsPublic
            };

            var result = await wishServiceClient.CreateWishAsync(request, cancellationToken);
            if (result.IsFailed)
            {
                return result.ToResult();
            }

            navigationService.NavigateTo<WishPage>(result.Value.Id);
            return Result.Ok();
        });
    }
}