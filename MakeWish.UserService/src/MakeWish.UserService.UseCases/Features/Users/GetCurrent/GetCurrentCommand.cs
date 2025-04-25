using FluentResults;
using MakeWish.UserService.UseCases.Dto;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Users.GetCurrent;

public sealed record GetCurrentCommand : IRequest<Result<UserDto>>;