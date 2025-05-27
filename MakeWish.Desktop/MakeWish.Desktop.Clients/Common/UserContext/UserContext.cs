namespace MakeWish.Desktop.Clients.Common.UserContext;

internal sealed class UserContext : IUserContext
{
    public string? Token { get; private set; }
    public Guid? UserId { get; private set; }
    
    public void SetToken(string token)
    {
        Token = token;
    }

    public void SetUserId(Guid userId)
    {
        UserId = userId;
    }
}