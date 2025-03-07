namespace MakeWish.UserService.UseCases.Services;

public interface IPasswordService
{
    bool Verify(string password, string passwordHash);
    
    string GetHash(string password);
}