using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AgnesAIImageEdit.Services
{
    public static class KeyVault
    {
        private static string FilePath => Path.Combine(App.DataDir, "apikey.bin");

        public static bool HasKey()
        {
            try { return File.Exists(FilePath) && ReadKey().Length > 0; }
            catch { return false; }
        }

        public static void SaveKey(string key)
        {
            var bytes = Encoding.UTF8.GetBytes(key ?? "");
            var protectedBytes = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
            File.WriteAllBytes(FilePath, protectedBytes);
        }

        public static string ReadKey()
        {
            if (!File.Exists(FilePath)) return "";
            try
            {
                var protectedBytes = File.ReadAllBytes(FilePath);
                var bytes = ProtectedData.Unprotect(protectedBytes, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(bytes);
            }
            catch { return ""; }
        }

        public static void Clear()
        {
            if (File.Exists(FilePath)) File.Delete(FilePath);
        }
    }
}
