using PeerReview.MvcHotel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerReview.MvcHotel.Services
{
    public class LookupsService
    {
        private readonly ApiClient _api;
        public LookupsService(ApiClient api) { _api = api; }

        // Lookups
        public Task<List<Lookup>?> List()
            => _api.Get<List<Lookup>>("/api/Lookups");

        public Task<Lookup?> Get(string code)
            => _api.Get<Lookup>($"/api/Lookups/{code}");

        public async Task Create(LookupCreateDto dto)
            => (await _api.Post("/api/Lookups", dto)).EnsureSuccessStatusCode();

        public async Task Update(string code, LookupUpdateDto dto)
            => (await _api.Put($"/api/Lookups/{code}", dto)).EnsureSuccessStatusCode();

        public async Task Delete(string code)
            => (await _api.Delete($"/api/Lookups/{code}")).EnsureSuccessStatusCode();

        // SubLookups
        public Task<List<SubLookup>?> Sub(int id)
            => _api.Get<List<SubLookup>>($"/api/Lookups/{id}/sub");

        public async Task AddSub(int id, SubLookupCreateDto dto)
            => (await _api.Post($"/api/Lookups/{id}/sub", dto)).EnsureSuccessStatusCode();

        public async Task UpdateSub(int id, SubLookupUpdateDto dto)
            => (await _api.Put($"/api/Lookups/sub/{id}", dto)).EnsureSuccessStatusCode();

        public async Task DeleteSub(int id)
            => (await _api.Delete($"/api/Lookups/sub/{id}")).EnsureSuccessStatusCode();

        public Task<SubLookupUpdateDto?> GetSubById(int id)
            => _api.Get<SubLookupUpdateDto>($"/api/Lookups/{id}/getsub");
    }
}
