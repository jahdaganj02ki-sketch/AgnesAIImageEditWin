using System.Collections.Generic;
using System.IO;
using AgnesAIImageEdit.Models;
using AgnesAIImageEdit.Services;
using FluentAssertions;
using Xunit;

namespace AgnesAIImageEdit.Tests.Services
{
    public class HistoryStoreTests : IDisposable
    {
        private readonly string _testFile;

        public HistoryStoreTests()
        {
            _testFile = Path.Combine(Path.GetTempPath(), $"agnes_history_test_{Guid.NewGuid()}.json");
            HistoryStore.SetTestFile(_testFile);
        }

        public void Dispose()
        {
            if (File.Exists(_testFile))
                File.Delete(_testFile);
            HistoryStore.ResetTestFile();
        }

        [Fact]
        public void Add_And_Load_ReturnsPersistedItems()
        {
            var items = new List<HistoryItem>
            {
                new HistoryItem { Mode = "text", Model = "agnes-image-2.1-flash", Prompt = "test1", OutputPath = "out1.png" },
                new HistoryItem { Mode = "edit", Model = "agnes-image-2.0-flash", Prompt = "test2", OutputPath = "out2.png" }
            };

            foreach (var item in items)
                HistoryStore.Add(item);

            var loaded = HistoryStore.Load();

            loaded.Should().HaveCount(2);
            loaded[0].Prompt.Should().Be("test1");
            loaded[1].Prompt.Should().Be("test2");
        }

        [Fact]
        public void Load_EmptyFile_ReturnsEmptyList()
        {
            File.WriteAllText(_testFile, "[]");
            var loaded = HistoryStore.Load();

            loaded.Should().NotBeNull();
            loaded.Should().BeEmpty();
        }

        [Fact]
        public void Add_MoreThanMaxItems_RemovesOldest()
        {
            for (int i = 0; i < 55; i++)
            {
                HistoryStore.Add(new HistoryItem { Prompt = $"item {i}" });
            }

            var loaded = HistoryStore.Load();

            loaded.Should().HaveCount(50);
            loaded[0].Prompt.Should().Be("item 5");
        }
    }
}