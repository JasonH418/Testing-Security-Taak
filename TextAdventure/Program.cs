using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace TextAdventure;

public class Program
{
    private static string _jwtToken = "";
    private static bool _isAdmin = false;
    private const string ApiBase = "https://localhost:49399/api/auth";
    private const string RoomsBase = "https://localhost:49399/api/rooms";

    public static async Task Main()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        var client = new HttpClient(handler);

        Console.WriteLine("=== TEXT ADVENTURE ===");

        bool ingelogd = false;
        while (!ingelogd)
        {
            Console.WriteLine("1. Inloggen");
            Console.WriteLine("2. Registreren");
            Console.Write("Kies een optie: ");
            var keuze = Console.ReadLine()?.Trim();

            if (keuze == "1")
                ingelogd = await Login(client);
            else if (keuze == "2")
                await Register(client);
            else
                Console.WriteLine("Ongeldige keuze. Typ 1 of 2.");
        }

        var world = GameSetup.CreateWorld(_isAdmin);
        Console.WriteLine("\nWelkom bij de C# Text Adventure!");
        if (_isAdmin) Console.WriteLine("[ADMIN] Noclip actief!");
        world.CurrentRoom.ShowDescription(world.Inventory);

        while (!world.IsGameOver && !world.IsWon)
        {
            Console.Write("\n> ");
            var input = Console.ReadLine()?.ToLower().Split(' ');
            if (input == null || input.Length == 0) continue;

            string cmd = input[0];

            switch (cmd)
            {
                case "help":
                    Console.WriteLine("Commando's: help, look, inventory, go [n|e|s|w], take [item], fight, unlock, quit");
                    break;
                case "look":
                    world.CurrentRoom.ShowDescription(world.Inventory);
                    break;
                case "inventory":
                    Console.WriteLine($"Je draagt: {world.Inventory.GetDisplayList()}");
                    break;
                case "go" when input.Length > 1:
                    if (Enum.TryParse<Direction>(input[1], out var dir)) world.Move(dir);
                    else Console.WriteLine("Ongeldige richting.");
                    break;
                case "take" when input.Length > 1:
                    var item = world.CurrentRoom.TakeItem(input[1]);
                    if (item != null) world.Inventory.AddItem(item);
                    Console.WriteLine(item != null ? $"Je pakt: {item.Name}" : "Dat ligt hier niet.");
                    break;
                case "fight":
                    world.Fight();
                    break;
                case "unlock":
                    await UnlockRoom(client, world);
                    break;
                case "quit":
                    return;
                default:
                    Console.WriteLine("Onbekend commando. Typ 'help'.");
                    break;
            }
        }

        Console.WriteLine(world.IsWon ? "\n--- GEWONNEN ---" : "\n--- GAME OVER ---");
    }

    private static async Task UnlockRoom(HttpClient client, Building world)
    {
        if (world.CurrentRoom.RoomId == null)
        {
            Console.WriteLine("Deze kamer heeft geen versleutelde inhoud.");
            return;
        }

        try
        {
            var roomId = world.CurrentRoom.RoomId.Value;

            var ksResponse = await client.GetAsync($"{RoomsBase}/{roomId}/keyshare");
            if (!ksResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Keyshare ophalen mislukt.");
                return;
            }
            var ksJson = await ksResponse.Content.ReadAsStringAsync();
            var keyshare = JsonDocument.Parse(ksJson).RootElement.GetProperty("keyshare").GetString() ?? "";

            Console.Write("Voer de passphrase in: ");
            var passphrase = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(passphrase))
            {
                Console.WriteLine("Passphrase mag niet leeg zijn.");
                return;
            }

            var body = JsonSerializer.Serialize(new { roomId, keyshare, passphrase });
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var unlockResponse = await client.PostAsync($"{RoomsBase}/unlock", content);
            if (!unlockResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Ongeldige passphrase.");
                return;
            }

            var unlockJson = await unlockResponse.Content.ReadAsStringAsync();
            var kamerInhoud = JsonDocument.Parse(unlockJson).RootElement.GetProperty("content").GetString() ?? "";
            Console.WriteLine($"\nKamerinhoud: {kamerInhoud}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fout: {ex.Message}");
        }
    }

    private static async Task<bool> Login(HttpClient client)
    {
        Console.Write("Gebruikersnaam: ");
        var username = Console.ReadLine()?.Trim();
        Console.Write("Wachtwoord: ");
        var password = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Gebruikersnaam en wachtwoord mogen niet leeg zijn.");
            return false;
        }

        try
        {
            var body = JsonSerializer.Serialize(new { username, password });
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{ApiBase}/login", content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);
                _jwtToken = doc.RootElement.GetProperty("token").GetString() ?? "";

                var parts = _jwtToken.Split('.');
                if (parts.Length > 1)
                {
                    var payload = parts[1];
                    payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
                    var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(payload));
                    var tokenDoc = JsonDocument.Parse(decoded);
                    foreach (var prop in tokenDoc.RootElement.EnumerateObject())
                    {
                        if (prop.Name.Contains("role", StringComparison.OrdinalIgnoreCase))
                        {
                            _isAdmin = prop.Value.GetString()?.Equals("admin", StringComparison.OrdinalIgnoreCase) ?? false;
                            break;
                        }
                    }
                }

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _jwtToken);

                Console.WriteLine("Inloggen geslaagd!\n");
                return true;
            }
            else
            {
                var fout = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Inloggen mislukt: {fout}");
                return false;
            }
        }
        catch (Exception)
        {
            Console.WriteLine("Verbindingsfout met de API. Probeer opnieuw.");
            return false;
        }
    }

    private static async Task Register(HttpClient client)
    {
        Console.Write("Kies een gebruikersnaam: ");
        var username = Console.ReadLine()?.Trim();
        Console.Write("Kies een wachtwoord: ");
        var password = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Gebruikersnaam en wachtwoord mogen niet leeg zijn.");
            return;
        }

        try
        {
            var body = JsonSerializer.Serialize(new { username, password });
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{ApiBase}/register", content);
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(response.IsSuccessStatusCode ?
                "Registratie geslaagd! Je kan nu inloggen." :
                $"Registratie mislukt: {result}");
        }
        catch (Exception)
        {
            Console.WriteLine("Verbindingsfout met de API. Probeer opnieuw.");
        }
    }
}