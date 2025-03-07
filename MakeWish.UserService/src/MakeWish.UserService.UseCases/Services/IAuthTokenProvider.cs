using MakeWish.UserService.Models;

namespace MakeWish.UserService.UseCases.Services;

public interface IAuthTokenProvider
{
    string GenerateToken(User user);
}