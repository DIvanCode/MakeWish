using FluentResults;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.Delete;

public sealed record DeleteWishCommand(Guid Id) : IRequest<Result>;