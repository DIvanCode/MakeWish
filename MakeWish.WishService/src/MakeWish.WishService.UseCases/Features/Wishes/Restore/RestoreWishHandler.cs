using FluentResults;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UseCases.Dto;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.Restore;

public sealed class RestoreWishHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    : IRequestHandler<RestoreWishCommand, Result<WishDto>>
{
    public async Task<Result<WishDto>> Handle(RestoreWishCommand request, CancellationToken cancellationToken)
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

        var restoreResult = wish.Restore(by: user);
        if (restoreResult.IsFailed)
        {
            return restoreResult;
        }

        var mainWishList = await unitOfWork.WishLists.GetMainForUserAsync(user, cancellationToken);
        var addResult = mainWishList.Add(wish, by: user);
        if (addResult.IsFailed)
        {
            return addResult;
        }
        
        unitOfWork.Wishes.Update(wish);
        unitOfWork.WishLists.AddWish(mainWishList, wish);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return WishDto.FromWish(wish, currUser: user);
    }
}