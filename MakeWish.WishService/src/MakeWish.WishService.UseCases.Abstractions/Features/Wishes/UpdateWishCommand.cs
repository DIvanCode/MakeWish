using FluentResults;
using MakeWish.WishService.UseCases.Abstractions.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Abstractions.Features.Wishes;

public sealed record UpdateWishCommand(Guid Id, string Title, string Description, bool IsPublic)
    : IRequest<Result<WishDto>>;