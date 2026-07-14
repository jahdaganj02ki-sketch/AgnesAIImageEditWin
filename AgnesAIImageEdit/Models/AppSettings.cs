using System;
using System.IO;
using System.Text.Json;

namespace AgnesAIImageEdit.Models
{
    public class AppSettings
    {
        public const string DefaultBaseUrl = "https://apihub.agnes-ai.com/v1";
        public const string ModelImage21 = "agnes-image-2.1-flash";
        public const string ModelImage20 = "agnes-image-2.0-flash";
        public const string ModelText = "agnes-2.0-flash";

        public string ApiBaseUrl { get; set; } = DefaultBaseUrl;
        public string DefaultImageModel { get; set; } = ModelImage21;
        public bool EnhancePrompt { get; set; } = true;
        public bool AlwaysSoftwareRender { get; set; } = false;
        public string OutputTier { get; set; } = "2K";            // used by agnes-image-2.1-flash
        public string OutputRatio { get; set; } = "1:1";          // used by agnes-image-2.1-flash
        public string OutputSizeExact { get; set; } = "1024x1024"; // used by agnes-image-2.0-flash
        public string SaveFolder { get; set; } = "";               // empty => DataDir/Outputs

        public static AppSettings Current { get; set; } = new AppSettings();

        private static string FilePath => Path.Combine(App.DataDir, "settings.json");

        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var json = File.ReadAllText(FilePath);
                    var s = JsonSerializer.Deserialize<AppSettings>(json);
                    if (s != null) return s;
                }
            }
            catch { }
            return new AppSettings();
        }

        public void Save()
        {
            try
            {
                var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilePath, json);
            }
            catch { }
        }

        public string ResolveSaveFolder() =>
            string.IsNullOrWhiteSpace(SaveFolder)
                ? Path.Combine(App.DataDir, "Outputs")
                : SaveFolder;
    }
}
