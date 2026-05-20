using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace TextAdventure;

public class Program
{
    private static string _jwtToken = "";
    private const string ApiBase = "https://localhost:49399/api/auth";

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
            {
                ingelogd = await Login(client);
            }
            else if (keuze == "2")
            {
                await Register(client);
            }
            else
            {
                Console.WriteLine("Ongeldige keuze. Typ 1 of 2.");
            }
        }

        var world = GameSetup.CreateWorld();
        Console.WriteLine("\nWelkom bij de C# Text Adventure!");
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
                    Console.WriteLine("Commando's: help, look, inventory, go [n|e|s|w], take [item], fight, quit");
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
                case "quit":
                    return;
                default:
                    Console.WriteLine("Onbekend commando. Typ 'help'.");
                    break;
            }
        }

        Console.WriteLine(world.IsWon ? "\n--- GEWONNEN ---" : "\n--- GAME OVER ---");
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