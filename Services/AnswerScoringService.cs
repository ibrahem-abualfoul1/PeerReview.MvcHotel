
using PeerReview.MvcHotel.Models;

namespace PeerReview.MvcHotel.Services
{
    public class AnswerScoringService
    {
        private readonly ApiClient _api;
        public AnswerScoringService(ApiClient api){ _api = api; }
        public Task<List<AnswerForScoringDto>?> ByUserScoring(int userId) => _api.Get<List<AnswerForScoringDto>>($"/api/AnswerScoring/by-user-unscored?userId={userId}");
        public Task<List<WithUnScoredAnswersDto>?> UsersWithunScoredAnswers() => _api.Get<List<WithUnScoredAnswersDto>>($"/api/AnswerScoring/users-with-unscored-answers");
        public Task<List<AllScoresDto>?> AllScores()
            => _api.Get<List<AllScoresDto>>("/api/AnswerScoring/all-scores");
        public async Task AddScore(ScorePostDto req) { (await _api.Post("/api/AnswerScoring/add-score", req)).EnsureSuccessStatusCode(); }
        public Task<List<ReviewersSummaryDto>?> ReviewersSummary()
    => _api.Get<List<ReviewersSummaryDto>>("/api/AnswerScoring/reviewers-summary");


    }
}

