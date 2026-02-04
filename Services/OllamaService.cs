using Microsoft.Extensions.Options;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.ViewModels;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Symphony.Portal.Web.Services
{
    public interface IOllamaService
    {
        /// <summary>
        /// Generate a chat response from AI
        /// </summary>
        Task<string> GenerateResponseAsync(string prompt, string systemPrompt = "");

        /// <summary>
        /// Generate quiz questions from content
        /// </summary>
        Task<QuizGenerationResult> GenerateQuestionsAsync(string content, int count = 5, string difficulty = "Medium");

        /// <summary>
        /// Check if Ollama is available
        /// </summary>
        Task<bool> IsAvailableAsync();
    }

    public class OllamaService : IOllamaService
    {
        private readonly HttpClient _httpClient;
        private readonly OllamaSettings _settings;
        private readonly ILogger<OllamaService> _logger;

        public OllamaService(HttpClient httpClient, IOptions<OllamaSettings> settings, ILogger<OllamaService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;

            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
        }

        public async Task<bool> IsAvailableAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/tags");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ollama service is not available");
                return false;
            }
        }

        public async Task<string> GenerateResponseAsync(string prompt, string systemPrompt = "")
        {
            try
            {
                var defaultSystemPrompt = @"Bạn là trợ lý AI của Symphony Portal - một hệ thống quản lý giáo dục.
Nhiệm vụ của bạn là hỗ trợ học viên về:
- Thông tin khóa học và đăng ký
- Lịch học và lịch thi
- Thủ tục hành chính
- Hỗ trợ kỹ thuật

Hãy trả lời ngắn gọn, thân thiện bằng tiếng Việt.
Nếu không biết câu trả lời, hãy đề nghị liên hệ Admin.";

                var request = new OllamaGenerateRequest
                {
                    Model = _settings.Model,
                    Prompt = prompt,
                    System = string.IsNullOrEmpty(systemPrompt) ? defaultSystemPrompt : systemPrompt,
                    Stream = false
                };

                var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/generate", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Ollama API error: {StatusCode}", response.StatusCode);
                    return "Xin lỗi, AI đang gặp sự cố. Vui lòng thử lại sau.";
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OllamaGenerateResponse>(responseJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result?.Response ?? "Không nhận được phản hồi từ AI.";
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("Ollama request timed out");
                return "AI mất quá nhiều thời gian để trả lời. Vui lòng thử câu hỏi ngắn hơn.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Ollama API");
                return "Đã xảy ra lỗi khi kết nối với AI. Vui lòng thử lại sau.";
            }
        }

        public async Task<QuizGenerationResult> GenerateQuestionsAsync(string content, int count = 5, string difficulty = "Medium")
        {
            try
            {
                var systemPrompt = @"Bạn là một giáo viên chuyên tạo câu hỏi trắc nghiệm.
Hãy tạo câu hỏi dựa trên nội dung được cung cấp.

QUAN TRỌNG: Trả về JSON theo đúng format sau, KHÔNG thêm bất kỳ text nào khác:
{
  ""questions"": [
    {
      ""content"": ""Nội dung câu hỏi?"",
      ""difficulty"": ""Easy|Medium|Hard"",
      ""options"": [
        {""content"": ""Đáp án A"", ""isCorrect"": false},
        {""content"": ""Đáp án B"", ""isCorrect"": true},
        {""content"": ""Đáp án C"", ""isCorrect"": false},
        {""content"": ""Đáp án D"", ""isCorrect"": false}
      ]
    }
  ]
}

Mỗi câu hỏi phải có đúng 4 đáp án, với 1 đáp án đúng.";

                var prompt = $@"Tạo {count} câu hỏi trắc nghiệm ở mức độ {difficulty} từ nội dung sau:

{content}

Trả về JSON theo format đã chỉ định.";

                var request = new OllamaGenerateRequest
                {
                    Model = _settings.Model,
                    Prompt = prompt,
                    System = systemPrompt,
                    Stream = false
                };

                var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/generate", httpContent);

                if (!response.IsSuccessStatusCode)
                {
                    return new QuizGenerationResult
                    {
                        Success = false,
                        Error = $"Ollama API error: {response.StatusCode}"
                    };
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                var ollamaResult = JsonSerializer.Deserialize<OllamaGenerateResponse>(responseJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (string.IsNullOrEmpty(ollamaResult?.Response))
                {
                    return new QuizGenerationResult
                    {
                        Success = false,
                        Error = "Không nhận được phản hồi từ AI"
                    };
                }

                // Parse the JSON response from AI
                var questions = ParseQuestionsFromAIResponse(ollamaResult.Response);

                return new QuizGenerationResult
                {
                    Success = true,
                    Questions = questions
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating questions");
                return new QuizGenerationResult
                {
                    Success = false,
                    Error = ex.Message
                };
            }
        }

        private List<GeneratedQuestion> ParseQuestionsFromAIResponse(string response)
        {
            try
            {
                // Try to find JSON in the response
                var jsonStart = response.IndexOf('{');
                var jsonEnd = response.LastIndexOf('}');

                if (jsonStart >= 0 && jsonEnd > jsonStart)
                {
                    var jsonStr = response.Substring(jsonStart, jsonEnd - jsonStart + 1);
                    var parsed = JsonSerializer.Deserialize<QuizJsonResponse>(jsonStr, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return parsed?.Questions ?? new List<GeneratedQuestion>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse AI response as JSON");
            }

            return new List<GeneratedQuestion>();
        }
    }

    // Request/Response models for Ollama API
    public class OllamaGenerateRequest
    {
        public string Model { get; set; } = string.Empty;
        public string Prompt { get; set; } = string.Empty;
        public string System { get; set; } = string.Empty;
        public bool Stream { get; set; } = false;
    }

    public class OllamaGenerateResponse
    {
        public string Model { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public bool Done { get; set; }
    }

    public class QuizJsonResponse
    {
        public List<GeneratedQuestion> Questions { get; set; } = new();
    }
}
