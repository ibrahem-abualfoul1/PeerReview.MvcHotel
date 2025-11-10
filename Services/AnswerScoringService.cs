using PeerReview.MvcHotel.Models;

namespace PeerReview.MvcHotel.Services
{
    public class AnswerScoringService
    {
        private readonly ApiClient _api;
        public AnswerScoringService(ApiClient api) { _api = api; }

        public Task<List<AnswerForScoringDto>?> ByUserScoringUnscored(int userId)
            => _api.Get<List<AnswerForScoringDto>>($"/api/AnswerScoring/by-user-unscored?userId={userId}");

        public Task<List<AnswerForScoringDto>?> ByUserScoringScored(int userId)
            => _api.Get<List<AnswerForScoringDto>>($"/api/AnswerScoring/by-user-scored-all?userId={userId}");

        public async Task AddScore(ScorePostDto req)
            => (await _api.Post("/api/AnswerScoring/add-score", req)).EnsureSuccessStatusCode();

        public async Task UpdateScore(ScoreUpdateDto req)
    => (await _api.Put("/api/AnswerScoring/by-user-scored/batch-update", req)).EnsureSuccessStatusCode();


        public Task<ReviewerUsersOverviewDto> UsersScoredStatus()
            => _api.Get<ReviewerUsersOverviewDto>("/api/AnswerScoring/users-scored-status");
    }
}
