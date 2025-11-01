
using PeerReview.MvcHotel.Models;

namespace PeerReview.MvcHotel.Services
{
    public class QuestionsService
    {
        private readonly ApiClient _api;
        public QuestionsService(ApiClient api){ _api = api; }

        public Task<List<QuestionDto>?> List() => _api.Get<List<QuestionDto>>("/api/Questions");
        public Task<QuestionDto?> Get(int id) => _api.Get<QuestionDto>($"/api/Questions/{id}");
        public Task<QuestionTypeDto?> GetType(int id) => _api.Get<QuestionTypeDto>($"/api/Questions/QuestionType");
        public async Task Create(QuestionCreateDto dto){ (await _api.Post("/api/Questions", dto)).EnsureSuccessStatusCode(); }
        public async Task Update(int id, QuestionUpdateDto dto){ (await _api.Put($"/api/Questions/{id}", dto)).EnsureSuccessStatusCode(); }
        public async Task Delete(int id){ (await _api.Delete($"/api/Questions/{id}")).EnsureSuccessStatusCode(); }
    }
}
