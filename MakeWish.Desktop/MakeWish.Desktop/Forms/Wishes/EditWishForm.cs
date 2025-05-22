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

public sealed partial class EditWishForm : Form
{
    private readonly IWishServiceClient _wishServiceClient;
    
    [ObservableProperty]
    private Guid _id = Guid.Empty;
    
    [ObservableProperty]
    private string _title = string.Empty;
    
    [ObservableProperty]
    private string _description = string.Empty;
    
    [ObservableProperty]
    private bool _isPublic;
    
    public EditWishForm(
        INavigationService navigationService,
        IRequestExecutor requestExecutor,
        IWishServiceClient wishServiceClient,
        Guid wishId)
        : base(navigationService, requestExecutor)
    {
        _wishServiceClient = wishServiceClient;

        LoadData(wishId);
    }

    [RelayCommand]
    private void Close()
    {
        NavigationService.CloseLastOverlay();
    }

    [RelayCommand]
    private void Save()
    {
        RequestExecutor.Execute(async () =>
        {
            var result = await UpdateAsync(Id, Title, Description, IsPublic);
            if (result.IsFailed)
            {
                return result.ToResult();
            }

            NavigationService.NavigateTo<WishPage>(result.Value.Id);
            return Result.Ok();
        });
    }

    private void LoadData(Guid id)
    {
        RequestExecutor.Execute(async () => await LoadDataAsync(id));
    }
    
    private async Task<Result> LoadDataAsync(Guid id)
    {
        var result = await _wishServiceClient.GetWishAsync(id, CancellationToken.None);
        if (result.IsFailed)
        {
            return result.ToResult();
        }
        
        var wish = result.Value;
        Id = wish.Id;
        Title = wish.Title;
        Description = wish.Description;
        IsPublic = wish.IsPublic;

        return Result.Ok();
    }

    private async Task<Result<Wish>> UpdateAsync(Guid id, string title, string description, bool isPublic)
    {
        var request = new UpdateWishRequest
        {
            Id = id,
            Title = title,
            Description = description,
            IsPublic = isPublic
        };

        return await _wishServiceClient.UpdateWishAsync(request, CancellationToken.None);
    }
}