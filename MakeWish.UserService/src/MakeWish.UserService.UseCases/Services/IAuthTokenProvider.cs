using MakeWish.UserService.Models.Entities;

namespace MakeWish.UserService.UseCases.Services;

public interface IAuthTokenProvider
{
    string GenerateToken(User user);
}