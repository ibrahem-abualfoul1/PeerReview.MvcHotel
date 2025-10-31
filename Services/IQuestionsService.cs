using PeerReview.MvcHotel.Models;

namespace PeerReview.MvcHotel.Services
{
    public interface IQuestionsService
    {

        Task<SurveyViewModel> GetSurveyAsync();
        Task SaveAnswersAsync(string userId, IDictionary<int, object?> answers);
    }
}
