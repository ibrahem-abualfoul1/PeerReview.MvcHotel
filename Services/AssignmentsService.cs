
using PeerReview.MvcHotel.Models;

namespace PeerReview.MvcHotel.Services
{
    public class AssignmentsService
    {
        private readonly ApiClient _api;
        public AssignmentsService(ApiClient api) { _api = api; }

        public Task<List<AssignmentDto>?> ByUser(int userId) => _api.Get<List<AssignmentDto>>($"/api/Assignments/by-user/{userId}");
        public Task<List<AssignmentDto>?> ByQuestion(int questionId) => _api.Get<List<AssignmentDto>>($"/api/Assignments/by-question/{questionId}");
        public async Task BulkAssign(AssignRequest req) { (await _api.Post("/api/Assignments/bulk", req)).EnsureSuccessStatusCode(); }
        public async Task Activate(int id) { (await _api.Post<object>($"/api/Assignments/{id}/activate", new { })).EnsureSuccessStatusCode(); }
        public async Task Deactivate(int id) { (await _api.Post<object>($"/api/Assignments/{id}/deactivate", new { })).EnsureSuccessStatusCode(); }

        public Task<List<AssignmentDto>?> GetAll() => _api.Get<List<AssignmentDto>>($"/api/Assignments/GetAll");

    }
}
