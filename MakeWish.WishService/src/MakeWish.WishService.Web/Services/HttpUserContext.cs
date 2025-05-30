using System.Security.Claims;
using EnsureThat;
using MakeWish.WishService.UseCases.Abstractions.Services;
using MakeWish.WishService.Web.Options;
using Microsoft.Extensions.Options;

namespace MakeWish.WishService.Web.Services;

public sealed class HttpUserContext : IUserContext
{
    private readonly HttpUserContextOptions _options;
    private readonly IEnumerable<Claim> _userClaims;

    public HttpUserContext(IHttpContextAccessor httpContextAccessor, IOptions<HttpUserContextOptions> options)
    {
        EnsureArg.IsNotNull(httpContextAccessor.HttpContext, nameof(httpContextAccessor.HttpContext));
        
        _options = options.Value;
        _userClaims = httpContextAccessor.HttpContext.User.Claims;
        
        httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var values);
        var token = values.FirstOrDefault(value =>
                     value?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                     ?? false)
                 ?? string.Empty;
        Token = string.Join("", token.Skip("Bearer ".Length));
    }
    
    public bool IsAuthenticated => _userClaims.Any(claim => claim.Type == _options.IdClaimType);
    
    public bool IsAdmin => _userClaims.Any(claim => claim.Type == _options.IsAdminClaimType && claim.Value == true.ToString());
    
    public Guid UserId => Guid.Parse(_userClaims.First(claim => claim.Type == _options.IdClaimType).Value);

    public string Token { get; }
}