using FluentResults;

namespace MakeWish.UserService.Utils.Errors;

public sealed class AuthenticationError : Error
{
    public AuthenticationError()
        : base("Authentication error")
    {
    }
}