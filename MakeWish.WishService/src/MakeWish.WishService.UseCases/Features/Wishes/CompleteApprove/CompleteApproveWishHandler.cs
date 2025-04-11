using FluentResults;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UseCases.Dto;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.CompleteApprove;

public sealed class CompleteApproveWishHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    : IRequestHandler<CompleteApproveWishCommand, Result<WishDto>>
{
    public async Task<Result<WishDto>> Handle(CompleteApproveWishCommand request, CancellationToken cancellationToken)
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
        
        var wish = await unitOfWork.Wishes.GetByIdAsync(request.WishId, cancellationToken);
        if (wish is null)
        {
            return new EntityNotFoundError(nameof(Wish), nameof(Wish.Id), request.WishId);
        }

        var completeApproveResult = wish.CompleteApprove(by: user);
        if (completeApproveResult.IsFailed)
        {
            return completeApproveResult;
        }
        
        unitOfWork.Wishes.Update(wish);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return WishDto.FromWish(wish, currUser: user);
    }
}