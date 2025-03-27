using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists.GetAll;

public sealed record GetAllWishListsCommand(bool OnlyMy): IRequest<Result<List<WishListDto>>>;