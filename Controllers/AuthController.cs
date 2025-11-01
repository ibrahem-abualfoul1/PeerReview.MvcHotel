using Microsoft.Extensions.Localization;

using Microsoft.AspNetCore.Mvc;
using PeerReview.MvcHotel.Models;
using PeerReview.MvcHotel.Services;
using PeerReview.MvcHotel.Resources;

namespace PeerReview.MvcHotel.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _auth;
        private readonly IStringLocalizer<SharedResource> _L;

        public AuthController(AuthService auth, IStringLocalizer<SharedResource> L){
            _L = L; _auth = auth; }

        [HttpGet] public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest model, string? returnUrl = null)
        {
            try{
                var res = await _auth.Login(model);
                TempData["msg"] = _L["Msg_Welcome"] + " " + (res?.userName ?? "");
                if (!string.IsNullOrEmpty(returnUrl)) return Redirect(returnUrl);
                return RedirectToAction("Index","Home");
            }catch(Exception ex){
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        public IActionResult Logout(){ _auth.Logout(); return RedirectToAction(nameof(Login)); }
    }
}
