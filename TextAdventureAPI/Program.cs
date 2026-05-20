using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TextAdventureAPI.Models;
using TextAdventureAPI.Services;

namespace TextAdventureAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var jwtKey = Encoding.UTF8.GetBytes("supersecretkey12345supersecretkey12345"); 

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            builder.Services.AddAuthorization();
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IEncryptionService, EncryptionService>();
            builder.Services.AddSingleton<IRoomService, RoomService>();
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

            app.MapPost("/api/auth/register", (RegisterRequest request, IAuthService authService) =>
            {
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                    return Results.BadRequest("Gebruikersnaam en wachtwoord zijn verplicht.");

                var success = authService.Register(request.Username, request.Password);
                if (!success) return Results.Conflict("Gebruiker bestaat al.");

                return Results.Ok($"Gebruiker '{request.Username}' geregistreerd.");
            });

            app.MapPost("/api/auth/login", (LoginRequest request, IAuthService authService) =>
            {
                var result = authService.Login(request.Username, request.Password);

                if (result == null) return Results.Unauthorized();
                if (result == "locked") return Results.Json(new { error = "Account is geblokkeerd na 3 foute pogingen." }, statusCode: 403);
                if (result == "invalid") return Results.Unauthorized();

                var token = GenerateJwt(request.Username, jwtKey);
                return Results.Ok(new { token });
            });
            
            app.MapPost("/api/encryption/encrypt", (EncryptRequest request, IEncryptionService encryptionService) =>
            {
                try
                {
                    var encrypted = encryptionService.Encrypt(request.Plaintext);
                    return Results.Ok(new { encryptedBase64 = encrypted });
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).RequireAuthorization();

            app.MapPost("/api/encryption/decrypt", (DecryptRequest request, IEncryptionService encryptionService) =>
            {
                try
                {
                    var plaintext = encryptionService.Decrypt(request.EncryptedBase64);
                    return Results.Ok(new { plaintext });
                }
                catch
                {
                    return Results.BadRequest("Decryptie mislukt.");
                }
            }).RequireAuthorization();
            
            app.MapGet("/api/rooms/{roomId}/keyshare", (int roomId, IRoomService roomService) =>
            {
                try
                {
                    var keyshare = roomService.GetKeyshare(roomId);
                    return Results.Ok(new { keyshare });
                }
                catch (Exception ex)
                {
                    return Results.NotFound(ex.Message);
                }
            }).RequireAuthorization();
            
            app.MapPost("/api/rooms/unlock", (UnlockRequest request, IRoomService roomService) =>
            {                
                var keyshare = roomService.GetKeyshare(request.RoomId);
                var content = roomService.UnlockRoom(request.RoomId, keyshare, request.Passphrase);

                if (content == null)
                    return Results.BadRequest("Ongeldige passphrase.");

                return Results.Ok(new { content });
            }).RequireAuthorization();

            app.Run();
        }

        private static string GenerateJwt(string username, byte[] jwtKey)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([new Claim(ClaimTypes.Name, username)]),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(jwtKey),
                    SecurityAlgorithms.HmacSha256Signature)
            });
            return handler.WriteToken(token);
        }
    }
}