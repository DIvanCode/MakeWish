using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.Get;

public sealed class GetWishHandler : IRequestHandler<GetWishCommand, Result<WishDto>>
{
    public async Task<Result<WishDto>> Handle(GetWishCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}