using FluentResults;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.Delete;

public sealed class DeleteWishHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteWishCommand, Result>
{
    public async Task<Result> Handle(DeleteWishCommand request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            return new AuthenticationError();
        }

        var user = await unitOfWork.Users.GetByIdAsync(userContext.UserId, cancellationToken);
        if (user is null)
        {
            return new EntityNotFoundError(nameof(User), nameof(User.Id), userContext.UserId);
        }
        
        var wish = await unitOfWork.Wishes.GetByIdAsync(request.Id, cancellationToken);
        if (wish is null)
        {
            return new EntityNotFoundError(nameof(Wish), nameof(Wish.Id), request.Id);
        }

        var deleteResult = wish.Delete(by: user);
        if (deleteResult.IsFailed)
        {
            return deleteResult;
        }
        
        var mainWishList = await unitOfWork.WishLists.GetMainForUserAsync(user, cancellationToken);
        var removeResult = mainWishList.Remove(wish, by: user);
        if (removeResult.IsFailed)
        {
            return removeResult;
        }
        
        unitOfWork.Wishes.Update(wish);
        unitOfWork.WishLists.RemoveWish(mainWishList, wish);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}