using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists.GetAll;

public sealed record GetAllWishListsCommand: IRequest<Result<List<WishListDto>>>;