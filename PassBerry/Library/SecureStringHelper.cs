namespace PassBerry.Library
{
    using System.Runtime.InteropServices;
    using System.Security;

    public class SecureStringHelper
    {
        public static SecureString GetSecureStringFromString(string value)
        {
            var password = new SecureString();
            var pass = value.ToCharArray();
            for (var i = 0; i < pass.Length; i++)
            {
                password.AppendChar(pass[i]);
            }
            return password;
        }

        public static string GetStringFromSecureString(SecureString value)
        {
            var bstr = Marshal.SecureStringToBSTR(value);
            try
            {
                return Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                Marshal.FreeBSTR(bstr);
            }
        }
    }
}