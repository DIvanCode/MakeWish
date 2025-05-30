using System.Security.Claims;
using EnsureThat;
using MakeWish.UserService.UseCases.Services;
using MakeWish.UserService.Web.Options;
using Microsoft.Extensions.Options;

namespace MakeWish.UserService.Web.Services;

public sealed class HttpUserContext : IUserContext
{
    private readonly HttpUserContextOptions _options;
    private readonly IEnumerable<Claim> _userClaims;
    
    public HttpUserContext(IHttpContextAccessor httpContextAccessor, IOptions<HttpUserContextOptions> options)
    {
        EnsureArg.IsNotNull(httpContextAccessor.HttpContext, nameof(httpContextAccessor.HttpContext));
        
        _userClaims = httpContextAccessor.HttpContext.User.Claims;
        _options = options.Value;
    }
    
    public bool IsAuthenticated => _userClaims.Any(claim => claim.Type == _options.IdClaimType);
    
    public bool IsAdmin => _userClaims.Any(claim => claim.Type == _options.IsAdminClaimType && claim.Value == true.ToString());
    
    public Guid UserId => Guid.Parse(_userClaims.Single(claim => claim.Type == _options.IdClaimType).Value);
}