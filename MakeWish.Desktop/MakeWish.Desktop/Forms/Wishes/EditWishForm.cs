using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Clients.WishService.Requests.Wishes;
using MakeWish.Desktop.Pages.Wishes;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Forms.Wishes;

internal sealed partial class EditWishForm(
    INavigationService navigationService,
    IOverlayService overlayService,
    IAsyncExecutor asyncExecutor,
    IWishServiceClient wishServiceClient,
    Guid id)
    : OverlayBase
{
    [ObservableProperty]
    private string _title = string.Empty;
    
    [ObservableProperty]
    private string _description = string.Empty;
    
    [ObservableProperty]
    private bool _isPublic;

    public override async Task<Result> LoadDataAsync(CancellationToken cancellationToken)
    {
        var result = await wishServiceClient.GetWishAsync(id, cancellationToken);
        if (result.IsFailed)
        {
            return result.ToResult();
        }
        
        Title = result.Value.Title;
        Description = result.Value.Description;
        IsPublic = result.Value.IsPublic;

        return Result.Ok();
    }
    
    [RelayCommand]
    private void Close()
    {
        overlayService.Close();
    }

    [RelayCommand]
    private void Save()
    {
        asyncExecutor.Execute(async cancellationToken =>
        {
            var request = new UpdateWishRequest
            {
                Id = id,
                Title = Title,
                Description = Description,
                IsPublic = IsPublic
            };
            
            var result = await wishServiceClient.UpdateWishAsync(request, cancellationToken);
            if (result.IsFailed)
            {
                return result.ToResult();
            }

            navigationService.NavigateTo<WishPage>(result.Value.Id);
            return Result.Ok();
        });
    }
}