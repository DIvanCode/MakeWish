using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.Complete;

public sealed record CompleteWishCommand(Guid WishId) : IRequest<Result<WishDto>>;