using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.GetPromisedWishes;

public sealed class GetPromisedWishesHandler
    : IRequestHandler<GetPromisedWishesCommand, Result<List<WishDto>>>
{
    public async Task<Result<List<WishDto>>> Handle(
        GetPromisedWishesCommand request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException(); 
    }
}