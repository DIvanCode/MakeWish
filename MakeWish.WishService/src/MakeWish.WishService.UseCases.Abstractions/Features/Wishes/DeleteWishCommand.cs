using FluentResults;
using MediatR;

namespace MakeWish.WishService.UseCases.Abstractions.Features.Wishes;

public sealed record DeleteWishCommand(Guid Id) : IRequest<Result>;