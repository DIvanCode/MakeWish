using System.Security.Claims;
using EnsureThat;
using MakeWish.WishService.UseCases.Services;
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
        
        _userClaims = httpContextAccessor.HttpContext.User.Claims;
        _options = options.Value;
    }
    
    public bool IsAuthenticated => _userClaims.Any(claim => claim.Type == _options.IdClaimType);
    
    public Guid UserId => Guid.Parse(_userClaims.First(claim => claim.Type == _options.IdClaimType).Value);
}