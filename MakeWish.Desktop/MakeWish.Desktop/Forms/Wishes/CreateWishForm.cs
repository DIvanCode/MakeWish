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

public sealed partial class CreateWishForm : Form
{
    private readonly IWishServiceClient _wishServiceClient;
    
    [ObservableProperty]
    private string _title = string.Empty;
    
    [ObservableProperty]
    private string _description = string.Empty;
    
    [ObservableProperty]
    private bool _isPublic;
    
    public CreateWishForm(
        INavigationService navigationService,
        IRequestExecutor requestExecutor,
        IWishServiceClient wishServiceClient)
        : base(navigationService, requestExecutor)
    {
        _wishServiceClient = wishServiceClient;
    }

    [RelayCommand]
    private void Close()
    {
        NavigationService.CloseLastOverlay();
    }

    [RelayCommand]
    private void Create()
    {
        RequestExecutor.Execute(async () =>
        {
            var result = await CreateAsync(Title, Description, IsPublic);
            if (result.IsFailed)
            {
                return result.ToResult();
            }

            NavigationService.NavigateTo<WishPage>(result.Value.Id);
            return Result.Ok();
        });
    }

    private async Task<Result<Wish>> CreateAsync(string title, string description, bool isPublic)
    {
        var request = new CreateWishRequest
        {
            Title = title,
            Description = description,
            IsPublic = isPublic
        };

        return await _wishServiceClient.CreateWishAsync(request, CancellationToken.None);
    }
}