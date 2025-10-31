
using PeerReview.MvcHotel.Models;

namespace PeerReview.MvcHotel.Services
{
    public class AnswersService
    {
        private readonly ApiClient _api;
        public AnswersService(ApiClient api){ _api = api; }
        public Task<List<Answer>?> Mine() => _api.Get<List<Answer>>("/api/Answers/mine");
        public async Task Create(AnswerCreateDto dto){ (await _api.Post("/api/Answers", dto)).EnsureSuccessStatusCode(); }
        public async Task Update(int id, AnswerUpdateDto dto){ (await _api.Put($"/api/Answers/{id}", dto)).EnsureSuccessStatusCode(); }
        public async Task Delete(int id){ (await _api.Delete($"/api/Answers/{id}")).EnsureSuccessStatusCode(); }
        public async Task Upload(int questionId, int questionItemId, string fileName, byte[] bytes, string contentType = "application/octet-stream")
        {
            var fields = new Dictionary<string,string>{
                {"questionId", questionId.ToString()},
                {"questionItemId", questionItemId.ToString()}
            };
            (await _api.PostMultipart("/api/Answers/upload", fields, ("file", fileName, bytes, contentType))).EnsureSuccessStatusCode();
        }
        public async Task BulkCreate(int questionId, Dictionary<int, string?> values, CancellationToken ct = default)
        {
            foreach (var kv in values)
            {
                var dto = new Models.AnswerCreateDto { questionId = questionId, questionItemId = kv.Key, value = kv.Value };
                await Create(dto);
            }
        }
    }
}

