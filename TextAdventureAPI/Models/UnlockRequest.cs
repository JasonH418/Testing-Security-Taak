namespace TextAdventureAPI.Models
{
    public class UnlockRequest
    {
        public int RoomId { get; set; }
        public string Passphrase { get; set; } = string.Empty;
    }
}