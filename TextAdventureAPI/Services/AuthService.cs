using System.Security.Cryptography;
using System.Text;

namespace TextAdventureAPI.Services
{
    public class AuthService : IAuthService
    {
        private static readonly Dictionary<string, (string Hash, int Attempts, bool Locked)> Users = new();

        public bool Register(string username, string password)
        {
            if (Users.ContainsKey(username)) return false;
            Users[username] = (HashString(password), 0, false);
            return true;
        }

        public string? Login(string username, string password)
        {
            if (!Users.TryGetValue(username, out var user)) return null;
            if (user.Locked) return "locked";

            if (HashString(password) != user.Hash)
            {
                var newAttempts = user.Attempts + 1;
                var locked = newAttempts >= 3;
                Users[username] = (user.Hash, newAttempts, locked);
                return locked ? "locked" : "invalid";
            }

            Users[username] = (user.Hash, 0, false);
            return "success";
        }

        private static string HashString(string input)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }
    }
}