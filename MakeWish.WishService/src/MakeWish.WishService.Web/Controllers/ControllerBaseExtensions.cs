using FluentResults;
using MakeWish.WishService.Utils.Errors;
using Microsoft.AspNetCore.Mvc;

namespace MakeWish.WishService.Web.Controllers;

public static class ControllerBaseExtensions
{
    public static ActionResult HandleResult(this ControllerBase controllerBase, Result result)
        => result.IsSuccess ? new OkResult() : controllerBase.HandleError(result.Errors);
    
    public static ActionResult<TResult> HandleResult<TResult>(this ControllerBase controllerBase, Result<TResult> result)
        => result.IsSuccess ? new OkObjectResult(result.Value) : controllerBase.HandleError(result.Errors);
    
    private static ObjectResult HandleError(this ControllerBase controllerBase, IEnumerable<IError> errors)
    {
        var error = errors.FirstOrDefault();
        
        var statusCode = error switch
        {
            AuthenticationError => StatusCodes.Status401Unauthorized,
            ForbiddenError => StatusCodes.Status403Forbidden,
            EntityNotFoundError => StatusCodes.Status404NotFound,
            EntityAlreadyExistsError => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };
        
        return controllerBase.StatusCode(statusCode, error?.Message ?? "An error has occurred.");
    }
} 