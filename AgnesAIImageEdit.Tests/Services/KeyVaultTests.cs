using System.IO;
using AgnesAIImageEdit.Services;
using Xunit;

namespace AgnesAIImageEdit.Tests.Services
{
    public class KeyVaultTests : IDisposable
    {
        private readonly string _testPath;

        public KeyVaultTests()
        {
            _testPath = Path.Combine(Path.GetTempPath(), $"keyvault_test_{Guid.NewGuid()}.bin");
            KeyVault.SetTestPath(_testPath);
            KeyVault.Clear();
        }

        public void Dispose()
        {
            KeyVault.ResetTestPath();
            if (File.Exists(_testPath)) File.Delete(_testPath);
        }

        [Fact]
        public void HasKey_AfterSave_ReturnsTrue()
        {
            KeyVault.SaveKey("test-key-123");

            Assert.True(KeyVault.HasKey());
        }

        [Fact]
        public void HasKey_BeforeSave_ReturnsFalse()
        {
            Assert.False(KeyVault.HasKey());
        }

        [Fact]
        public void SaveKey_And_ReadKey_ReturnsSameValue()
        {
            KeyVault.SaveKey("my-secret-key");

            var result = KeyVault.ReadKey();

            Assert.Equal("my-secret-key", result);
        }

        [Fact]
        public void Clear_RemovesKey()
        {
            KeyVault.SaveKey("test");
            Assert.True(KeyVault.HasKey());

            KeyVault.Clear();

            Assert.False(KeyVault.HasKey());
            Assert.Equal("", KeyVault.ReadKey());
        }
    }
}