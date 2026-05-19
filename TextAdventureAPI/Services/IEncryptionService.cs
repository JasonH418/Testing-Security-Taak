namespace TextAdventureAPI.Services
{
    public interface IEncryptionService
    {
        string Encrypt(string plaintext);
        string Decrypt(string encryptedBase64);
    }
}