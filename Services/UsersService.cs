
using PeerReview.MvcHotel.Models;

namespace PeerReview.MvcHotel.Services
{
    public class UsersService
    {
        private readonly ApiClient _api;
        public UsersService(ApiClient api){ _api = api; }

        public Task<List<User>?> List() => _api.Get<List<User>>("/api/Users");
        public Task<User?> Get(int id) => _api.Get<User>($"/api/Users/{id}");
        public async Task Create(UserCreateDto dto){ (await _api.Post("/api/Users", dto)).EnsureSuccessStatusCode(); }
        public async Task Update(int id, UserUpdateDto dto){ (await _api.Put($"/api/Users/{id}", dto)).EnsureSuccessStatusCode(); }
        public async Task Delete(int id){ (await _api.Delete($"/api/Users/{id}")).EnsureSuccessStatusCode(); }
        public async Task Activate(int id){ (await _api.Post<object>($"/api/Users/{id}/activate", new { })).EnsureSuccessStatusCode(); }
        public async Task Deactivate(int id){ (await _api.Post<object>($"/api/Users/{id}/deactivate", new { })).EnsureSuccessStatusCode(); }
    }
}
