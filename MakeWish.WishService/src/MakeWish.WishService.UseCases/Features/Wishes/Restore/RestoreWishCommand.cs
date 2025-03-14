using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.Restore;

public sealed record RestoreWishCommand(Guid Id) : IRequest<Result<WishDto>>;