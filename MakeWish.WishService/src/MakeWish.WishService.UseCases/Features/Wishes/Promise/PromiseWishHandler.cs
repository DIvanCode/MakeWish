using FluentResults;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UseCases.Dto;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.Promise;

public sealed class PromiseWishHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    : IRequestHandler<PromiseWishCommand, Result<WishDto>>
{
    public async Task<Result<WishDto>> Handle(PromiseWishCommand request, CancellationToken cancellationToken)
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

        var promiseResult = wish.PromiseBy(user);
        if (promiseResult.IsFailed)
        {
            return promiseResult;
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