namespace MakeWish.UserService.UseCases.Services;

public interface IUserContext
{
    bool IsAuthenticated { get; }
    int UserId { get; }
}