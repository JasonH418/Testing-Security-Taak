namespace TextAdventureAPI.Services
{
    public interface IRoomService
    {
        string GetKeyshare(int roomId);
        string? UnlockRoom(int roomId, string keyshare, string passphrase);
    }
}