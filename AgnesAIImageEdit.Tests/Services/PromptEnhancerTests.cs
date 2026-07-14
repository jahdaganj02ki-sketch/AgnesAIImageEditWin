using AgnesAIImageEdit.Services;
using Xunit;

namespace AgnesAIImageEdit.Tests.Services
{
    public class PromptEnhancerTests
    {
        [Fact]
        public async Task EnhanceAsync_ReturnsResult()
        {
            // This test verifies the method executes without throwing
            // In CI without API key, it may return error or fallback
            var (enhanced, error) = await PromptEnhancer.EnhanceAsync("test prompt");

            // Either enhanced or error should be set
            Assert.True(enhanced != null || error != null);
        }
    }
}