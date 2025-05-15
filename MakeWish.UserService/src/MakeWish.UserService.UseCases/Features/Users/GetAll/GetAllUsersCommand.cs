using FluentResults;
using MakeWish.UserService.UseCases.Dto;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Users.GetAll;

public sealed record GetAllUsersCommand(string? Query, bool? OnlyFriends) : IRequest<Result<List<UserDto>>>;