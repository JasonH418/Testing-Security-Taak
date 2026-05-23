using System;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.IO;

var store = new X509Store(StoreLocation.CurrentUser);
store.Open(OpenFlags.ReadOnly);
var cert = store.Certificates.Find(X509FindType.FindBySubjectName, "localhost", false)[0];
store.Close();

void SaveEnc(string plaintext, string filename) {
    var cms = new EnvelopedCms(new ContentInfo(Encoding.UTF8.GetBytes(plaintext)));
    cms.Encrypt(new CmsRecipient(SubjectIdentifierType.IssuerAndSerialNumber, cert));
    File.WriteAllText(filename, Convert.ToBase64String(cms.Encode()));
    Console.WriteLine("Saved: " + filename);
}

SaveEnc("Je bevindt je in de geheime wapenkamer. Je vindt een magisch zwaard!", "../room1.enc");
SaveEnc("Je bevindt je in de schatkamer. Je vindt de eindbaas sleutel!", "../room2.enc");
