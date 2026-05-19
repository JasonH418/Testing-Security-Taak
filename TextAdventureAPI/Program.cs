using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace TextAdventureAPI;

public class Program
{
    private static readonly Dictionary<string, string> _users = new();
    private static readonly Dictionary<string, int> _failedAttempts = new();
    private static readonly Dictionary<string, bool> _lockedAccounts = new();

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET")
                        ?? "supersecretkey12345textadventure";

        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "textadventure-api",
                    ValidAudience = "textadventure-client",
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey))
                };
            });

        builder.Services.AddAuthorization();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

            var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapPost("/api/auth/register", (User user) =>
        {
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
                return Results.BadRequest("Gebruikersnaam en wachtwoord zijn verplicht.");

            if (_users.ContainsKey(user.Username))
                return Results.BadRequest("Gebruiker bestaat al.");

            _users[user.Username] = HashString(user.Password);
            return Results.Ok("Registratie geslaagd!");
        })
        .WithName("RegisterUser")
        .WithOpenApi();

        app.MapPost("/api/auth/login", (User user) =>
        {
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
                return Results.BadRequest("Gebruikersnaam en wachtwoord zijn verplicht.");

            if (_lockedAccounts.TryGetValue(user.Username, out bool locked) && locked)
                return Results.BadRequest("Account is geblokkeerd na te veel foute pogingen.");

            if (!_users.TryGetValue(user.Username, out string? storedHash))
                return Results.Unauthorized();

            if (storedHash != HashString(user.Password))
            {
                _failedAttempts.TryGetValue(user.Username, out int attempts);
                attempts++;
                _failedAttempts[user.Username] = attempts;

                if (attempts >= 3)
                {
                    _lockedAccounts[user.Username] = true;
                    return Results.BadRequest("Account is geblokkeerd na te veel foute pogingen.");
                }

                return Results.Unauthorized();
            }

            _failedAttempts[user.Username] = 0;

            var token = GenerateJwtToken(user.Username, secretKey);
            return Results.Ok(new { token });
        })
        .WithName("LoginUser")
        .WithOpenApi();

        app.MapGet("/api/game/secret", () => "Je bent ingelogd!")
            .RequireAuthorization()
            .WithName("SecretEndpoint")
            .WithOpenApi();

        app.Run();
    }

    private static string HashString(string input)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }

    private static string GenerateJwtToken(string username, string secretKey)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(ClaimTypes.Name, username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "textadventure-api",
            audience: "textadventure-client",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}