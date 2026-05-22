using System;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TextAdventureAPI.Services
{
    public class EncryptionService : IEncryptionService
    {
        public string Encrypt(string plaintext)
        {
            var cert = LoadCert();
            if (cert == null) throw new Exception("Geen certificaat gevonden.");
            var cms = new EnvelopedCms(new ContentInfo(Encoding.UTF8.GetBytes(plaintext)));
            cms.Encrypt(new CmsRecipient(SubjectIdentifierType.IssuerAndSerialNumber, cert));
            return Convert.ToBase64String(cms.Encode());
        }

        public string Decrypt(string encryptedBase64)
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