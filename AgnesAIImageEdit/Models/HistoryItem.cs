using System;
using System.Collections.Generic;

namespace AgnesAIImageEdit.Models
{
    public class HistoryItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Mode { get; set; } = "edit"; // edit | text
        public string Model { get; set; } = "";
        public string Prompt { get; set; } = "";
        public string? InputThumbBase64 { get; set; }
        public string OutputPath { get; set; } = "";
    }
}
