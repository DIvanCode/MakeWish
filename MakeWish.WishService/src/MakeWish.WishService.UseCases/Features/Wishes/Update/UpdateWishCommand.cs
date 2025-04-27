using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.Update;

public sealed record UpdateWishCommand(Guid Id, string Title, string Description, string? ImageUrl)
    : IRequest<Result<WishDto>>;