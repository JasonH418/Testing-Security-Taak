namespace TextAdventureAPI.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EncryptedContent { get; set; } = string.Empty;
        public bool IsEncrypted { get; set; }
    }
}