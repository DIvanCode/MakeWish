using FluentResults;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UseCases.Dto;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.PromiseCancel;

public sealed class PromiseCancelWishHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    : IRequestHandler<PromiseCancelWishCommand, Result<WishDto>>
{
    public async Task<Result<WishDto>> Handle(PromiseCancelWishCommand request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            return new AuthenticationError();
        }
        
        var wish = await unitOfWork.Wishes.GetByIdAsync(request.WishId, cancellationToken);
        if (wish is null)
        {
            return new EntityNotFoundError(nameof(Wish), nameof(Wish.Id), request.WishId);
        }

        var user = await unitOfWork.Users.GetByIdAsync(userContext.UserId, cancellationToken);
        if (user is null)
        {
            return new EntityNotFoundError(nameof(User), nameof(User.Id), userContext.UserId);
        }

        var promiseCancelResult = wish.PromiseCancelBy(user);
        if (promiseCancelResult.IsFailed)
        {
            return promiseCancelResult;
        }
        
        unitOfWork.Wishes.Update(wish);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new WishDto(
            wish.Id,
            wish.Title,
            wish.Description,
            wish.Status.ToString(),
            wish.Owner.Id,
            wish.Promiser?.Id,
            wish.Completer?.Id);
    }
}