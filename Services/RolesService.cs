
using PeerReview.MvcHotel.Models;

namespace PeerReview.MvcHotel.Services
{
    public class RolesService
    {
        private readonly ApiClient _api;
        public RolesService(ApiClient api){ _api = api; }

        public Task<List<Role>?> List() => _api.Get<List<Role>>("/api/Roles");
       
    }
}
