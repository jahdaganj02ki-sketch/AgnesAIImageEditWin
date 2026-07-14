using System.Windows.Media.Imaging;

namespace AgnesAIImageEdit.Models
{
    public interface IChatItem { }

    public class UserPromptItem : IChatItem
    {
        public string Prompt { get; set; } = "";
        public BitmapImage? PreviewImage { get; set; }
        public bool HasImage => PreviewImage != null;
    }

    public class StatusItem : IChatItem
    {
        public string Text { get; set; } = "";
    }

    public class ResultItem : IChatItem
    {
        public BitmapImage? OutputImage { get; set; }
        public string Prompt { get; set; } = "";
        public string Model { get; set; } = "";
        public string OutputPath { get; set; } = "";
        public string? RevisedPrompt { get; set; }
        public int? Rating { get; set; } // 1 up, -1 down, null
    }
}
