using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists.GetUsersWithAccess;

public sealed record GetUsersWithAccessToWishListCommand(Guid Id) : IRequest<Result<List<UserDto>>>;