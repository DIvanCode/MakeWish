namespace MakeWish.Desktop.Clients.UserContext;

public class UserContext : IUserContext
{
    public bool IsAuthenticated => !string.IsNullOrEmpty(Token) && UserId != null;
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

    public void Clear()
    {
        Token = null;
        UserId = null;
    }
} 