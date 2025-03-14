using FluentResults;

namespace MakeWish.WishService.Utils.Errors;

public sealed class AuthenticationError : Error
{
    public AuthenticationError()
        : base("Authentication error")
    {
    }
}