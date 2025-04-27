using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.PromiseCancel;

public sealed record PromiseCancelWishCommand(Guid Id): IRequest<Result<WishDto>>;