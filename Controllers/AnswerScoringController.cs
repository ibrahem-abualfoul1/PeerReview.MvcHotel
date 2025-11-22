using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using PeerReview.MvcHotel.Models;
using PeerReview.MvcHotel.Resources;
using PeerReview.MvcHotel.Services;

namespace PeerReview.MvcHotel.Controllers
{
    [Route("[controller]")]
    public class AnswerScoringController : Controller
    {
        private readonly AnswerScoringService _svc;
        private readonly IStringLocalizer<SharedResource> _L;

        public AnswerScoringController(AnswerScoringService svc, IStringLocalizer<SharedResource> l)
        {
            _svc = svc;
            _L = l;
        }

        // GET /AnswerScoring
        [HttpGet("")]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var users = await _svc.UsersScoredStatus() ?? new ReviewerUsersOverviewDto();
            return View(users);
        }

        // GET /AnswerScoring/users/5/unscored
        [HttpGet("users/{userId:int}/unscored")]
        public async Task<IActionResult> ByUserUnscored(int userId, CancellationToken ct)
        {
            var answers = await _svc.ByUserScoringUnscored(userId) ?? new List<AnswerForScoringDto>();
            return View("ByUserScoringUnscored", answers);
        }

        // GET /AnswerScoring/users/5/scored
        [HttpGet("users/{userId:int}/scored")]
        public async Task<IActionResult> ByUserScored(int userId, CancellationToken ct)
        {
            var answers = await _svc.ByUserScoringScored(userId) ?? new List<AnswerForScoringDto>();
            ViewBag.UserId = userId; // <-- ضفها
            return View("ByUserScoringScored", answers);
        }


        // If submitting from Razor FORM -> keep POST
        // POST /AnswerScoring/add
        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddScore([FromForm] ScorePostDto req, CancellationToken ct)
        {
            await _svc.AddScore(req);
            TempData["Toast"] = _L["ScoreAdded"].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("/AnswerScoring/update-form", Name = "AnswerScoring_UpdateForm")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateForm([FromForm] ScoreUpdateDto req, CancellationToken ct)
        {
            if (req.Items == null || req.Items.Count == 0)
            {
                TempData["Toast"] = "لا يوجد درجات لتحديثها.";
                // الأحسن ترجع Redirect بدل View(Index(ct))
                return RedirectToAction(nameof(Index));
            }

            await _svc.UpdateScore(req);

            TempData["Toast"] = _L["ScoreUpdated"].Value;  

            // نفس الكلام هنا
            return RedirectToAction(nameof(Index));
        }



    }
}
