using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.Get;

public sealed record GetWishCommand(Guid WishId) : IRequest<Result<WishDto>>;