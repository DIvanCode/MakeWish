using FluentResults;
using MakeWish.UserService.UseCases.Dto;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Users.GetAll;

public sealed record GetAllUsersCommand : IRequest<Result<List<UserDto>>>;