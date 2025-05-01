using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MakeWish.UserService.Models;
using MakeWish.UserService.Models.Entities;
using MakeWish.UserService.UseCases.Services;
using MakeWish.UserService.Web.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MakeWish.UserService.Web.Services;

public sealed class JwtTokenProvider(
    IOptions<JwtTokenOptions> jwtTokenOptions,
    IOptions<HttpUserContextOptions> httpUserContextOptions) : IAuthTokenProvider
{
    private readonly JwtTokenOptions _jwtTokenOptions = jwtTokenOptions.Value;
    private readonly HttpUserContextOptions _httpUserContextOptions = httpUserContextOptions.Value;

    public string GenerateToken(User user)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenOptions.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var claims = new Claim[]
        {
            new(_httpUserContextOptions.IdClaimType, user.Id.ToString())
        };
        
        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials,
            expires: DateTime.UtcNow.AddHours(_jwtTokenOptions.ExpiresIntervalHours));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}