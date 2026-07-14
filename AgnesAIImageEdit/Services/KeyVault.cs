using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AgnesAIImageEdit.Services
{
    public static class KeyVault
    {
        private static string FilePath => Path.Combine(App.DataDir, "apikey.bin");
        private static string? _testFilePath;

        public static bool HasKey()
        {
            try { return File.Exists(GetPath()) && ReadKey().Length > 0; }
            catch { return false; }
        }

        public static void SaveKey(string key)
        {
            var bytes = Encoding.UTF8.GetBytes(key ?? "");
            var protectedBytes = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
            File.WriteAllBytes(GetPath(), protectedBytes);
        }

        public static string ReadKey()
        {
            if (!File.Exists(GetPath())) return "";
            try
            {
                var protectedBytes = File.ReadAllBytes(GetPath());
                var bytes = ProtectedData.Unprotect(protectedBytes, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(bytes);
            }
            catch { return ""; }
        }

        public static void Clear()
        {
            if (File.Exists(GetPath())) File.Delete(GetPath());
        }

        private static string GetPath() => _testFilePath ?? FilePath;
        public static void SetTestPath(string path) => _testFilePath = path;
        public static void ResetTestPath() => _testFilePath = null;
    }
}
