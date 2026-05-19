using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TextAdventureAPI.Services
{
    public class RoomService : IRoomService
    {
        private static readonly Dictionary<int, string> RoomPassphrases = new()
        {
            { 1, "dragonslayer" },
            { 2, "shadowkey" }
        };
       
        private static readonly Dictionary<int, string> RoomKeyshares = new()
        {
            { 1, "KEYSHARE-ROOM1-ABC123" },
            { 2, "KEYSHARE-ROOM2-XYZ789" }
        };
        
        private static readonly Dictionary<int, string> EncryptedRooms = new()
        {
            { 1, EncryptContent("Je bevindt je in de geheime wapenkamer. Je vindt een magisch zwaard!") },
            { 2, EncryptContent("Je bevindt je in de schatkamer. Je vindt de eindbaas sleutel!") }
        };

        public string GetKeyshare(int roomId)
        {
            if (!RoomKeyshares.ContainsKey(roomId))
                throw new Exception($"Kamer {roomId} niet gevonden.");
            return RoomKeyshares[roomId];
        }

        public string? UnlockRoom(int roomId, string keyshare, string passphrase)
        {
            if (!RoomPassphrases.ContainsKey(roomId)) return null;
            if (keyshare != RoomKeyshares[roomId]) return null;
            if (passphrase != RoomPassphrases[roomId]) return null;

            return DecryptContent(EncryptedRooms[roomId]);
        }

        private static string EncryptContent(string plaintext)
        {
            var cert = LoadCert();
            if (cert == null) throw new Exception("Geen certificaat gevonden.");
            var cms = new EnvelopedCms(new ContentInfo(Encoding.UTF8.GetBytes(plaintext)));
            cms.Encrypt(new CmsRecipient(SubjectIdentifierType.IssuerAndSerialNumber, cert));
            return Convert.ToBase64String(cms.Encode());
        }

        private static string DecryptContent(string encryptedBase64)
        {
            var cert = LoadCert();
            if (cert == null) throw new Exception("Geen certificaat gevonden.");
            var cms = new EnvelopedCms();
            cms.Decode(Convert.FromBase64String(encryptedBase64));
            cms.Decrypt(new X509Certificate2Collection(cert));
            return Encoding.UTF8.GetString(cms.ContentInfo.Content);
        }

        private static X509Certificate2? LoadCert()
        {
            var store = new X509Store(StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            foreach (var cert in store.Certificates)
                if (cert.HasPrivateKey) { store.Close(); return cert; }
            store.Close();
            return null;
        }
    }
}