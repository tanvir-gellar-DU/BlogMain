
using BlogManagement.API.Models.DTOs.Ai;
using BlogManagement.API.Service.Interface;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BlogManagement.API.Services
{
    public class AiService : IAiService
    {
        private const int MaxPromptInputCharacters = 12000;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> EnhanceContentAsync(AiEnhanceRequest request)
        {
            var prompt = BuildEnhancePrompt(request);
            return await CallAiApiAsync(prompt);
        }

        public async Task<string> ChatAsync(AiChatRequest request)
        {
            var prompt = BuildChatPrompt(request);
            return await CallAiApiAsync(prompt);
        }

        private string BuildEnhancePrompt(AiEnhanceRequest request)
        {
            var sb = new StringBuilder();
            // sb.AppendLine("You are a professional writing assistant. Enhance the following blog content.");
            //sb.AppendLine("Improve grammar, clarity, and flow while preserving the original meaning and tone.");

            if (!string.IsNullOrEmpty(request.Tone))
                sb.AppendLine($"Adjust the tone to be: {request.Tone}");

            if (request.ImproveGrammar) sb.AppendLine("You are a grammar and spelling correction tool STRICT RULES:- Fix ONLY spelling mistakes and grammatical errors . Do NOT add new sentences, paragraphs, or ideas. Do NOT remove any existing content. Do NOT rephrase or reword sentences unless they are grammatically broken. Do NOT improve vocabulary or style.Do NOT add introductions, conclusions, or transitions. Preserve the author's original tone, structure, and voice exactly . Return ONLY the corrected text, nothing else — no explanations, no commentsIf a sentence is grammatically correct, leave it completely untouched.");
            if (request.ImproveClarity) sb.AppendLine("- Improve clarity and readability only where the sentence is hard to understand. Do NOT rewrite clear sentences.");
            if (request.ImproveFlow) sb.AppendLine("- Improve transitions between sentences only where flow is broken. Do NOT restructure or reorder content.Try to Make a better flow in a context like.");

            sb.AppendLine();
            sb.AppendLine("Original content:");
            sb.AppendLine(LimitInput(request.Content));
            sb.AppendLine();
            sb.AppendLine("Enhanced version:");

            return sb.ToString();
        }

        private static string BuildChatPrompt(AiChatRequest request)
        {
            var sb = new StringBuilder();
            sb.AppendLine("You are an AI writing assistant helping with a blog post.");

            if (!string.IsNullOrEmpty(request.BlogContent))
            {
                sb.AppendLine("Current blog content:");
                sb.AppendLine(LimitInput(request.BlogContent));
                sb.AppendLine();
            }

            sb.AppendLine("User message: " + LimitInput(request.Message));
            return sb.ToString();
        }

        private static string LimitInput(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            value = value.Trim();
            return value.Length <= MaxPromptInputCharacters
                ? value
                : value[..MaxPromptInputCharacters];
        }

        private async Task<string> CallAiApiAsync(string prompt)
        {
            var aiSettings = _configuration.GetSection("Ai");
            var provider = aiSettings["Provider"]?.ToLower() ?? "openai";
            var apiKey = aiSettings["ApiKey"];
            var model = aiSettings["Model"] ?? "gpt-3.5-turbo";

            if (string.IsNullOrEmpty(apiKey))
                return "AI service is not configured. Please set up your API key in appsettings.json.";

            try
            {
                var requestBody = provider switch
                {
                    "openai" => new
                    {
                        model,
                        messages = new[] { new { role = "user", content = prompt } },
                        max_tokens = 2000,
                        temperature = 0.7
                    },
                    _ => new
                    {
                        model,
                        messages = new[] { new { role = "user", content = prompt } },
                        max_tokens = 2000,
                        temperature = 0.7
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var apiUrl = provider switch
                {
                    "openai" => "https://api.openai.com/v1/chat/completions",
                    "groq" => "https://api.groq.com/openai/v1/chat/completions",
                    _ => aiSettings["Endpoint"] ?? "https://api.openai.com/v1/chat/completions"
                };

                var response = await _httpClient.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseJson);
                var result = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return result ?? "No response from AI.";
            }
            catch (Exception ex)
            {
                return $"AI service error: {ex.Message}";
            }
        }
    }
}
