using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using AgnesAIImageEdit.Models;

namespace AgnesAIImageEdit.Services
{
    public static class HistoryStore
    {
        private static string FilePath => Path.Combine(App.DataDir, "history.json");
        private const int MaxItems = 50;

        public static List<HistoryItem> Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var json = File.ReadAllText(FilePath);
                    var list = JsonSerializer.Deserialize<List<HistoryItem>>(json);
                    if (list != null) return list;
                }
            }
            catch { }
            return new List<HistoryItem>();
        }

        public static void Save(List<HistoryItem> items)
        {
            try
            {
                var json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilePath, json);
            }
            catch { }
        }

        public static void Add(HistoryItem item)
        {
            var items = Load();
            items.Add(item);
            while (items.Count > MaxItems) items.RemoveAt(0);
            Save(items);
        }
    }
}
