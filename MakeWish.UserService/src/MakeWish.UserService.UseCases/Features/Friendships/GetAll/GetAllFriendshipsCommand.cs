using FluentResults;
using MakeWish.UserService.UseCases.Dto;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Friendships.GetAll;

public sealed record GetAllFriendshipsCommand : IRequest<Result<List<FriendshipDto>>>;