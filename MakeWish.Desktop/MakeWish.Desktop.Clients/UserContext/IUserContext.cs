namespace MakeWish.Desktop.Clients.UserContext;

public interface IUserContext
{
    bool IsAuthenticated { get; }
    string? Token { get; }
    Guid? UserId { get; }
    
    void SetToken(string token);
    void SetUserId(Guid userId);
    void Clear();
} 