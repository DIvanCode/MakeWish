using FluentResults;

namespace MakeWish.UserService.Utils.Errors;

public sealed class BadRequestError : Error
{
    public BadRequestError(string message)
        : base(message)
    {
    }
}