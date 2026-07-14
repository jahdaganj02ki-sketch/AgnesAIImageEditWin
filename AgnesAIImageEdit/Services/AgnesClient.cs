using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AgnesAIImageEdit.Services
{
    public class AgnesClient
    {
        private static readonly HttpClient _http = new HttpClient { Timeout = TimeSpan.FromSeconds(120) };

        private static string BaseUrl => AppSettings.Current.ApiBaseUrl.TrimEnd('/');

        public static async Task<(byte[]? PngBytes, string? RevisedPrompt, string? Error)> GenerateImageAsync(
            string model, string prompt, List<string>? images, string size, string? ratio, string responseFormat = "b64_json")
        {
            var url = $"{BaseUrl}/images/generations";

            var extraBody = new Dictionary<string, object?> { ["response_format"] = responseFormat };
            if (images != null && images.Count > 0) extraBody["image"] = images;

            var body = new Dictionary<string, object?>
            {
                ["model"] = model,
                ["prompt"] = prompt,
                ["size"] = size,
                ["extra_body"] = extraBody
            };
            if (!string.IsNullOrEmpty(ratio)) body["ratio"] = ratio;

            try
            {
                var req = new HttpRequestMessage(HttpMethod.Post, url);
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", KeyVault.ReadKey());
                req.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

                using var resp = await _http.SendAsync(req);
                var raw = await resp.Content.ReadAsStringAsync();
                if (!resp.IsSuccessStatusCode)
                    return (null, null, $"API error {(int)resp.StatusCode}: {raw}");

                using var doc = JsonDocument.Parse(raw);
                var root = doc.RootElement;
                if (!root.TryGetProperty("data", out var data) || data.GetArrayLength() == 0)
                    return (null, null, "No image returned by API.");

                var first = data[0];
                string? revised = first.TryGetProperty("revised_prompt", out var rp) && rp.ValueKind == JsonValueKind.String
                    ? rp.GetString()
                    : null;

                if (responseFormat == "b64_json")
                {
                    if (!first.TryGetProperty("b64_json", out var b64) || b64.ValueKind != JsonValueKind.String || string.IsNullOrEmpty(b64.GetString()))
                        return (null, null, "Empty b64_json in response.");
                    return (Convert.FromBase64String(b64.GetString()!), revised, null);
                }
                else
                {
                    var imgUrl = first.TryGetProperty("url", out var u) && u.ValueKind == JsonValueKind.String ? u.GetString() : null;
                    if (string.IsNullOrEmpty(imgUrl)) return (null, null, "Empty url in response.");
                    var bytes = await _http.GetByteArrayAsync(imgUrl);
                    return (bytes, revised, null);
                }
            }
            catch (TaskCanceledException)
            {
                return (null, null, "Request timed out after 120s.");
            }
            catch (Exception ex)
            {
                return (null, null, ex.Message);
            }
        }

        public static async Task<(string? Text, string? Error)> EnhancePromptAsync(string systemInstruction, string userPrompt)
        {
            var url = $"{BaseUrl}/chat/completions";

            var body = new Dictionary<string, object?>
            {
                ["model"] = AppSettings.ModelText,
                ["messages"] = new[]
                {
                    new Dictionary<string, string> { ["role"] = "system", ["content"] = systemInstruction },
                    new Dictionary<string, string> { ["role"] = "user", ["content"] = userPrompt }
                },
                ["temperature"] = 0.6,
                ["max_tokens"] = 512,
                ["stream"] = false
            };

            try
            {
                var req = new HttpRequestMessage(HttpMethod.Post, url);
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", KeyVault.ReadKey());
                req.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

                using var resp = await _http.SendAsync(req);
                var raw = await resp.Content.ReadAsStringAsync();
                if (!resp.IsSuccessStatusCode)
                    return (null, $"Enhance error {(int)resp.StatusCode}: {raw}");

                using var doc = JsonDocument.Parse(raw);
                var text = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();
                return (text, null);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }
    }
}
