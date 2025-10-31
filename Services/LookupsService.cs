
using PeerReview.MvcHotel.Models;

namespace PeerReview.MvcHotel.Services
{
    public class LookupsService
    {
        private readonly ApiClient _api;
        public LookupsService(ApiClient api){ _api = api; }
        public Task<List<Lookup>?> List() => _api.Get<List<Lookup>>("/api/Lookups");
        public Task<List<SubLookup>?> Sub(int id) => _api.Get<List<SubLookup>>($"/api/Lookups/{id}/sub");
    }
}
