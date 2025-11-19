using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using Microsoft.AspNetCore.Mvc;
using PeerReview.MvcHotel.Models;
using PeerReview.MvcHotel.Services;
using PeerReview.MvcHotel.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace PeerReview.MvcHotel.Controllers
{
    public class AuthController : Controller
    {
        private const string LoginAttemptSessionKey = "LoginAttempts";
        private const int MaxLoginAttempts = 5;

        private readonly AuthService _auth;
        private readonly IStringLocalizer<SharedResource> _L;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService auth, IStringLocalizer<SharedResource> L, ILogger<AuthController> logger)
        {
            _L = L; _auth = auth; _logger = logger;
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

            var attempts = HttpContext.Session.GetInt32(LoginAttemptSessionKey) ?? 0;
            if (attempts >= MaxLoginAttempts)
            {
                _logger.LogWarning("Login blocked due to too many attempts for user {User}", model.userName);
                ModelState.AddModelError(string.Empty, _L["Login_AttemptsExceeded"].Value);
                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && !Url.IsLocalUrl(returnUrl))
            {
                _logger.LogWarning("Rejected non-local returnUrl {ReturnUrl} for user {User}", returnUrl, model.userName);
                ModelState.AddModelError(string.Empty, _L["Login_InvalidReturnUrl"].Value);
                return View(model);
            }

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

                HttpContext.Session.Remove(LoginAttemptSessionKey);
                _logger.LogInformation("User {User} logged in successfully", res?.userName ?? model.userName);

                TempData["msg"] = SharedResource.Msg_Welcome + " " + (res?.userName ?? "");

                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return LocalRedirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                attempts++;
                HttpContext.Session.SetInt32(LoginAttemptSessionKey, attempts);
                _logger.LogWarning(ex, "Login failed for user {User}. Attempts: {Attempts}", model.userName, attempts);
                ModelState.AddModelError(string.Empty, _L["Login_Failed"].Value);
                return View(model);
            }
        }

        public IActionResult Logout() { _auth.Logout(); return RedirectToAction(nameof(Login)); }
    }
}
