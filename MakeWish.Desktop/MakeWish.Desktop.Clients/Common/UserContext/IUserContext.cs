namespace MakeWish.Desktop.Clients.Common.UserContext;

public interface IUserContext
{
    string? Token { get; }
    Guid? UserId { get; }
    
    void SetToken(string token);
    void SetUserId(Guid userId);
} 