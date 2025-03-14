using FluentResults;

namespace MakeWish.WishService.Utils.Errors;

public class ForbiddenError : Error
{
    public ForbiddenError(string message)
        : base(message)
    {
    }

    public ForbiddenError(string type, string operation)
        : base($"Forbidden to {operation} '{type}'")
    {
    }

    public ForbiddenError(string type, string operation, string field, object value)
        : base($"Forbidden to {operation} '{type}' with '{field}' = '{value}'")
    {
    }
}
