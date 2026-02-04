namespace Symphony.Portal.Web.Models.ViewModels
{
    /// <summary>
    /// ViewModel for AI-generated quiz questions
    /// </summary>
    public class GeneratedQuestion
    {
        public string Content { get; set; } = string.Empty;
        public string Difficulty { get; set; } = "Medium";
        public List<GeneratedOption> Options { get; set; } = new();
    }

    public class GeneratedOption
    {
        public string Content { get; set; } = string.Empty;
        public bool IsCorrect { get; set; } = false;
    }

    /// <summary>
    /// Request model for quiz generation
    /// </summary>
    public class QuizGenerationRequest
    {
        public string Content { get; set; } = string.Empty;
        public string SubjectId { get; set; } = string.Empty;
        public int QuestionCount { get; set; } = 5;
        public string Difficulty { get; set; } = "Medium"; // Easy, Medium, Hard
    }

    /// <summary>
    /// Response from AI for quiz generation
    /// </summary>
    public class QuizGenerationResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public List<GeneratedQuestion> Questions { get; set; } = new();
    }
}
