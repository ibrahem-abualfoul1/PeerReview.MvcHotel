
using PeerReview.MvcHotel.Models;
using System.Text.Json.Serialization;

namespace PeerReview.MvcHotel.Services
{
    public class UploadResultDto
    {
        [JsonPropertyName("answerId")]
        public int AnswerId { get; set; }

        [JsonPropertyName("fileId")]
        public int FileId { get; set; }

        [JsonPropertyName("answerFileId")]
        public int AnswerFileId { get; set; }

        [JsonPropertyName("fileName")]
        public string FileName { get; set; } = "";

        [JsonPropertyName("fileUrl")]
        public string FileUrl { get; set; } = "";
    }
    public class AnswersService
    {
        private readonly ApiClient _api;
        public AnswersService(ApiClient api) { _api = api; }
        public Task<List<Answer>?> Mine() => _api.Get<List<Answer>>("/api/Answers/mine");
        public async Task Create(List<AnswerCreateDto> dto) { (await _api.Post("/api/Answers", dto)).EnsureSuccessStatusCode(); }
        public async Task Update(int id, AnswerUpdateDto dto) { (await _api.Put($"/api/Answers/{id}", dto)).EnsureSuccessStatusCode(); }
        public async Task Delete(int id) { (await _api.Delete($"/api/Answers/{id}")).EnsureSuccessStatusCode(); }
        public async Task<UploadResultDto> Upload(
    int questionId,
    int questionItemId,
    string fileName,
    byte[] bytes,
    string contentType = "application/octet-stream")
        {
            var fields = new Dictionary<string, string>{
        {"questionId", questionId.ToString()},
        {"questionItemId", questionItemId.ToString()}
    };

            var response = await _api.PostMultipart(
                "/api/Answers/upload",
                fields,
                ("file", fileName, bytes, contentType)
            );

            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"Upload API failed ({(int)response.StatusCode} {response.StatusCode}): {body}");
            }

            // نفترض الـ body = JSON بنفس الشكل اللي رجعناه من الـ API
            return System.Text.Json.JsonSerializer.Deserialize<UploadResultDto>(body)!;
        }

        public async Task DeleteFile(int answerFileId)
        {
            var response = await _api.Delete($"/api/Answers/files/{answerFileId}");

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception(
                    $"Delete file API failed ({(int)response.StatusCode} {response.StatusCode}): {body}");
            }
        }



    }
}

