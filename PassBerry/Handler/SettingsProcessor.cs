namespace PassBerry.Handler
{
    using PassBerry.Library;
    using PassBerry.Properties;
    using System.Security.Cryptography.X509Certificates;

    internal class SettingsProcessor
    {
        private RSACryptoHelper rsa = new RSACryptoHelper();
        private static readonly string defaultPfxSubjectName = "Nap7.PassBerry.Default";
        private static readonly string defaultPfxPassword = "p@ssw0rd";
        private static SettingsProcessor singleton = new SettingsProcessor();

        private X509Certificate2 publicKey { get; set; }
        private X509Certificate2 privateKey { get; set; }

        private SettingsProcessor()
        {

            // 从配置文件读取 是否使用Default
            // 从配置文件读取私钥名称
        }

        public static SettingsProcessor GetInstance()
        {
            return singleton;
        }

        public void InstallDefaultPrivateCertificate()
        {
            rsa.InstallPrivateKeyCertificateToStore(Resources.Nap7_PassBerry_Default_pfx, defaultPfxPassword);
        }
    }
}