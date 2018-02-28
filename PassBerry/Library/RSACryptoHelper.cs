namespace PassBerry.Library
{
    using System;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;

    internal class RSACryptoHelper
    {

        // Generate RSA certificate by Makecert and pvk2pfx tools.
        // makecert -r -pe -sky Exchange -n "CN=Hendry Test" -sv private.pvk public.cer
        // pvk2pfx.exe -pvk private.pvk -pi "p@ssw0rd"  -spc public.cer -pfx private.pfx  -po "p@ssw0rd"

        // Usage:
        //var rsa = new Library.RSACryptoHelper();
        //var publicKey = rsa.LoadPublicKeyFromCerFile("public.cer");
        //rsa.CheckPrivateKeyCertificateName("Hendry Test");
        //var privateKey = rsa.GetPrivateKeyCertificate("Hendry Test");
        //var data1 = System.Text.Encoding.Default.GetBytes("Hello, World!");
        //var data2 = rsa.Encrypt(publicKey, data1);
        //var data3 = rsa.Decrypt(privateKey, data2);
        //var data4 = System.Text.Encoding.Default.GetString(data3);

        public byte[] LoadPublicKeyFromCerFile(string certificateFilePath)
        {
            var certificate = new X509Certificate2(certificateFilePath);
            return certificate.GetRawCertData();
        }

        public bool CheckPrivateKeyCertificateName(string certificateName)
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            var certificateCollection = store.Certificates.Find(X509FindType.FindBySubjectName, "CN = " + certificateName, true);
            store.Close();
            return certificateCollection.Count > 0;
        }

        public byte[] Encrypt(byte[] rawCertificateData, byte[] data)
        {
            var cert = new X509Certificate2(rawCertificateData);
            var rsa = (RSACryptoServiceProvider)cert.PublicKey.Key;
            return rsa.Encrypt(data, false);
        }

        public X509Certificate2 GetPrivateKeyCertificate(string certificateSubjectName)
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            foreach (var certificate in store.Certificates)
            {
                if (certificate.SubjectName.Name == "CN=" + certificateSubjectName)
                {
                    store.Close();
                    return certificate;
                }
            }
            store.Close();
            throw new Exception($"Cannot fine the specific certificate ({certificateSubjectName}) from the My/CurrentUser certificate store");
        }

        public byte[] Decrypt(X509Certificate2 privateKeyCertificate, byte[] data)
        {
            var rsa = (RSACryptoServiceProvider)privateKeyCertificate.PrivateKey;
            return rsa.Decrypt(data, false);
        }
    }
}