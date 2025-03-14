using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.GetWaitingApproveWishes;

public sealed class GetWaitingApproveWishesHandler
    : IRequestHandler<GetWaitingApproveWishesCommand, Result<List<WishDto>>>
{
    public async Task<Result<List<WishDto>>> Handle(
        GetWaitingApproveWishesCommand request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}