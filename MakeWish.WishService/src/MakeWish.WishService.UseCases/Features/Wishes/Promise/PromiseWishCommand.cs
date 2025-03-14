using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.Promise;

public sealed record PromiseWishCommand(Guid WishId): IRequest<Result<WishDto>>;