
using Newtonsoft.Json;
using PeerReview.MvcHotel.Models;

namespace PeerReview.MvcHotel.Services
{
    public class AuthService
    {
        private readonly ApiClient _api;
        private readonly IHttpContextAccessor _ctx;

        public AuthService(ApiClient api, IHttpContextAccessor ctx){ _api = api; _ctx = ctx; }

        public async Task<LoginResponse?> Login(LoginRequest req)
        {
            var res = await _api.Post("/api/Auth/login", req);
            var txt = await res.Content.ReadAsStringAsync();
            if (!res.IsSuccessStatusCode) throw new Exception($"Auth failed: {res.StatusCode} {txt}");
            var dto = JsonConvert.DeserializeObject<LoginResponse>(txt);
            if (dto?.token != null) _ctx.HttpContext?.Session.SetString("jwt", dto.token);
            _ctx.HttpContext?.Session.SetString("userName", dto?.userName ?? "");
            _ctx.HttpContext?.Session.SetString("role", dto?.role ?? "");
            return dto;
        }

        public void Logout()
        {
            _ctx.HttpContext?.Session.Remove("jwt");
            _ctx.HttpContext?.Session.Remove("userName");
            _ctx.HttpContext?.Session.Remove("role");
        }
    }
}
