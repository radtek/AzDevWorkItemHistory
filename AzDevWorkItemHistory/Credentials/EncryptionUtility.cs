using System;
using System.Security.Cryptography;
using System.Text;

namespace AzDevWorkItemHistory.Credentials
{
    public static class EncryptionUtility
    {
        private static readonly byte[] EntropyBytes = Encoding.UTF8.GetBytes(@"TKwHdxUHut59yGGaUqgmH65eqVB#^m6n9hu@fr4zehtR8J!DioUQp8DPF3S22$a0q9APXio96cM%hxQ7hrUmUW0ra4D8aGGHSVm");

        public static string EncryptString(string value)
        {
            var decryptedByteArray = Encoding.UTF8.GetBytes(value);
            var encryptedByteArray = ProtectedData.Protect(decryptedByteArray, EntropyBytes, DataProtectionScope.CurrentUser);
            var encryptedString = Convert.ToBase64String(encryptedByteArray);
            return encryptedString;
        }

        public static string DecryptString(string encryptedString)
        {
            var encryptedByteArray = Convert.FromBase64String(encryptedString);
            var decryptedByteArray = ProtectedData.Unprotect(encryptedByteArray, EntropyBytes, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(decryptedByteArray);
        }
    }
}