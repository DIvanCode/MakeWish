using FluentResults;
using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.Models;
using MakeWish.UserService.UseCases.Services;
using MakeWish.UserService.Utils.Errors;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Users.Authenticate;

public sealed class AuthenticateHandler(
    IUnitOfWork unitOfWork,
    IPasswordService passwordService,
    IAuthTokenProvider authTokenProvider)
    : IRequestHandler<AuthenticateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(AuthenticateCommand request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            return new EntityNotFoundError(nameof(User), nameof(User.Email), request.Email);
        }
        
        var isPasswordCorrect = passwordService.Verify(request.Password, user.PasswordHash);
        if (!isPasswordCorrect)
        {
            return new AuthenticationError();
        }
        
        var token = authTokenProvider.GenerateToken(user);
        
        return token;
    }
}