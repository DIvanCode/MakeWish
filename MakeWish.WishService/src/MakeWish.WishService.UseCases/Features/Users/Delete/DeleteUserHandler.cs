using FluentResults;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.Utils.Errors;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Users.Delete;

public sealed class DeleteUserHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteUserCommand, Result>
{
    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return new EntityNotFoundError(nameof(User), nameof(User.Id), request.UserId);
        }
        
        unitOfWork.Users.Remove(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Ok();
    }
}