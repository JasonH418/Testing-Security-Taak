namespace TextAdventure;
public class Building
{
    public Room CurrentRoom { get; private set; }
    public Inventory Inventory { get; } = new();
    public bool IsGameOver { get; private set; }
    public bool IsWon { get; set; }
    private readonly bool _isAdmin;
    public Building(Room startRoom, bool isAdmin = false)
    {
        CurrentRoom = startRoom;
        _isAdmin = isAdmin;
    }
    public void Move(Direction dir)
    {
        if (!CurrentRoom.Exits.TryGetValue(dir, out var nextRoom))
        {
            if (_isAdmin)
            {
                Console.WriteLine("[NOCLIP] Admin passeert de muur — maar er is geen kamer aan de andere kant.");
                return;
            }
            Console.WriteLine("Je kan die kant niet op.");
            return;
        }
        if (CurrentRoom.MonsterAlive)
        {
            Console.WriteLine("Je probeerde weg te rennen, maar het monster greep je!");
            IsGameOver = true;
            return;
        }
        if (nextRoom.RequiredItem != null && !Inventory.HasItem(nextRoom.RequiredItem))
        {
            if (_isAdmin)
            {
                Console.WriteLine("[NOCLIP] Admin passeert de vergrendelde deur!");
            }
            else
            {
                Console.WriteLine($"De deur zit op slot. Je hebt een {nextRoom.RequiredItem} nodig.");
                return;
            }
        }
        CurrentRoom = nextRoom;
        if (CurrentRoom.IsDeadly) IsGameOver = true;
        if (CurrentRoom.IsWin) IsWon = true;
        if (!IsGameOver && !IsWon) CurrentRoom.ShowDescription(Inventory);
        else Console.WriteLine(CurrentRoom.Description);
    }
    public void Fight()
    {
        if (!CurrentRoom.MonsterAlive)
        {
            Console.WriteLine("Er is hier niets om tegen te vechten.");
            return;
        }
        if (Inventory.HasItem("Zwaard"))
        {
            CurrentRoom.MonsterAlive = false;
            Console.WriteLine("Met je zwaard versla je het monster! De weg is veilig.");
        }
        else
        {
            Console.WriteLine("Zonder wapen ben je geen partij voor het monster...");
            IsGameOver = true;
        }
    }
}