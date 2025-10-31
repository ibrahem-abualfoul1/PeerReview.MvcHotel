
using PeerReview.MvcHotel.Models;
using System.Text.Json;

namespace PeerReview.MvcHotel.Services
{
    public class FileQuestionsService : IQuestionsService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<FileQuestionsService> _logger;

        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        public FileQuestionsService(IWebHostEnvironment env, ILogger<FileQuestionsService> logger)
        {
            _env = env;
            _logger = logger;
        }

        public async Task<SurveyViewModel> GetSurveyAsync()
        {
            // الملف: wwwroot/data/survey.json
            var path = Path.Combine(_env.WebRootPath, "data", "survey.json");
            if (!File.Exists(path))
                throw new FileNotFoundException($"survey.json not found at: {path}");

            await using var fs = File.OpenRead(path);
            var groups = await JsonSerializer.DeserializeAsync<List<QuestionGroup>>(fs, _jsonOpts)
                         ?? new List<QuestionGroup>();

            return new SurveyViewModel
            {
                HeaderTitle = "استطلاع الفنادق",
                HeaderDescription = "مساعدتك في تحسين خدماتنا",
                Groups = groups
            };
        }

        public async Task SaveAnswersAsync(string userId, IDictionary<int, object?> answers)
        {
            // حفظ بسيط على ملف JSON (تقدر تستبدله بقاعدة بيانات لاحقاً)
            var outDir = Path.Combine(_env.ContentRootPath, "App_Data", "SurveyResponses");
            Directory.CreateDirectory(outDir);

            var fileName = $"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{userId}.json";
            var path = Path.Combine(outDir, fileName);

            var payload = new
            {
                UserId = userId,
                SubmittedAtUtc = DateTime.UtcNow,
                Answers = answers
            };

            await File.WriteAllTextAsync(path, JsonSerializer.Serialize(payload, _jsonOpts));
            _logger.LogInformation("Survey answers saved to {Path}", path);
        }
    }
}

