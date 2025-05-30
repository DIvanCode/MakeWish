using FluentResults;
using MakeWish.WishService.UseCases.Abstractions.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Abstractions.Features.Wishes;

public sealed record GetUserWishesCommand(Guid UserId, string? Query) : IRequest<Result<IReadOnlyList<WishDto>>>;