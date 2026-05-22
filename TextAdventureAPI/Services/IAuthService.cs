namespace TextAdventureAPI.Services
{
    public interface IAuthService
    {
        bool Register(string username, string password, string role = "Player");
        string? Login(string username, string password);
        string GetRole(string username);
    }
}