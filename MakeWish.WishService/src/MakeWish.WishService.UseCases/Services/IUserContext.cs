namespace MakeWish.WishService.UseCases.Services;

public interface IUserContext
{
    bool IsAuthenticated { get; }
    Guid UserId { get; }
}