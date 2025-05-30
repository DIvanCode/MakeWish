namespace MakeWish.WishService.UseCases.Abstractions.Services;

public interface IUserContext
{
    bool IsAuthenticated { get; }
    bool IsAdmin { get; }
    Guid UserId { get; }
    string Token { get; }
}