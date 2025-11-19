using Microsoft.Extensions.Localization;

using Microsoft.AspNetCore.Mvc;
using PeerReview.MvcHotel.Models;
using PeerReview.MvcHotel.Services;
using PeerReview.MvcHotel.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace PeerReview.MvcHotel.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _auth;
        private readonly IStringLocalizer<SharedResource> _L;

        public AuthController(AuthService auth, IStringLocalizer<SharedResource> L)
        {
            _L = L; _auth = auth;
        }

        [AllowAnonymous]
        [HttpGet] 
        public IActionResult Login() => View();

        //[AllowAnonymous]
        //[HttpPost]
        //public async Task<IActionResult> Login(LoginRequest model, string? returnUrl = null)
        //{
        //    try{
        //        var res = await _auth.Login(model);
        //        TempData["msg"] = SharedResource.Msg_Welcome + " " + (res?.userName ?? "");
        //        if (!string.IsNullOrEmpty(returnUrl)) return Redirect(returnUrl);
        //        return RedirectToAction("Index","Home");
        //    }catch(Exception ex){
        //        ModelState.AddModelError(string.Empty, ex.Message);
        //        return View(model);
        //    }
        //}
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequest model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var res = await _auth.Login(model); 

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, res?.UserId?.ToString() ?? Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, res?.userName ?? model.userName ?? "")
        };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                    });

                TempData["msg"] = SharedResource.Msg_Welcome + " " + (res?.userName ?? "");

                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return LocalRedirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        public IActionResult Logout() { _auth.Logout(); return RedirectToAction(nameof(Login)); }
    }
}
