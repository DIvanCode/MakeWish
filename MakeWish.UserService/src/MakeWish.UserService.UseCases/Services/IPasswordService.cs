namespace MakeWish.UserService.UseCases.Services;

public interface IPasswordService
{
    bool Verify(string password, string passwordHash);
    
    string GetHash(string password);
}

public sealed class BCryptPasswordService : IPasswordService
{
    public bool Verify(string password, string passwordHash) => BCrypt.Net.BCrypt.Verify(password, passwordHash);
    
    public string GetHash(string password) => BCrypt.Net.BCrypt.HashPassword(password);
}