using System.Threading.Tasks;

namespace AgnesAIImageEdit.Services
{
    public static class PromptEnhancer
    {
        private const string SystemInstruction =
            "You are an expert image-editing prompt engineer for an AI image model. " +
            "Given a short, casual user instruction, rewrite it into a single detailed, " +
            "photorealistic editing prompt. Keep the user's intent, subject, and composition " +
            "unchanged. Include: what to change, what to preserve, target style, lighting, " +
            "composition, and quality. Output only the improved prompt, no commentary.";

        public static async Task<(string? Enhanced, string? Error)> EnhanceAsync(string userPrompt)
        {
            var (text, err) = await AgnesClient.EnhancePromptAsync(SystemInstruction, userPrompt);
            return (text, err);
        }
    }
}
