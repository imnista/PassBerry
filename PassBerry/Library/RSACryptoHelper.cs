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

        // 公钥和私钥都是采用 X509Certificate2 对象进行封装，并没有区别
        // X509Certificate2 类的构造方法允许通过 byte[] 类型的 certificateRawData 进行构造
        // 也可通过 直接指定 certificateFilePath 从文件进行加载

        //public X509Certificate2 GetCertificate(byte[] certificateRawData)
        //{
        //    return new X509Certificate2(certificateRawData);
        //}

        //public byte[] GetCertificateRawData(string certificateFilePath)
        //{
        //    var certificate = new X509Certificate2(certificateFilePath);
        //    return certificate.GetRawCertData();
        //}

        public void InstallPrivateKeyCertificateToStore(byte[] certificatePfxRawData, string pfxPassword)
        {
            var certificate = new X509Certificate2(certificatePfxRawData, pfxPassword, X509KeyStorageFlags.Exportable);
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            store.Add(certificate);
            store.Close();
        }

        public bool CheckPrivateKeyCertificateNameInStore(string certificateName)
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            var certificateCollection = store.Certificates.Find(X509FindType.FindBySubjectName, "CN = " + certificateName, true);
            store.Close();
            return certificateCollection.Count > 0;
        }

        public X509Certificate2 GetPrivateKeyCertificateFromStore(string certificateSubjectName)
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

        public byte[] Encrypt(X509Certificate2 publicCertificate, byte[] data)
        {
            var rsa = (RSACryptoServiceProvider)publicCertificate.PublicKey.Key;
            return rsa.Encrypt(data, false);
        }

        public byte[] Decrypt(X509Certificate2 privateKeyCertificate, byte[] data)
        {
            var rsa = (RSACryptoServiceProvider)privateKeyCertificate.PrivateKey;
            return rsa.Decrypt(data, false);
        }
    }
}