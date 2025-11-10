using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using PeerReview.MvcHotel.Models;
using PeerReview.MvcHotel.Resources;
using PeerReview.MvcHotel.Services;

namespace PeerReview.MvcHotel.Controllers
{
    public class AnswerScoringController : Controller
    {
        private readonly AnswerScoringService _svc;
        private readonly IStringLocalizer<SharedResource> _L;

        public AnswerScoringController(AnswerScoringService svc, IStringLocalizer<SharedResource> l )
        {
            _svc = svc;
            _L = l;
        }

        // GET /AnswerScoring
        // قائمة المستخدمين اللي عندهم UnscoredCount > 0
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _svc.UsersWithunScoredAnswers() ?? new List<WithUnScoredAnswersDto>();
            return View(users);
        }

        // GET /AnswerScoring/ByUser?userId=1
        // عرض كل الإجابات غير المقيّمة لمستخدم معيّن
        [HttpGet("ByUser")]
        public async Task<IActionResult> ByUser(int userId)
        {
            var answers = await _svc.ByUserScoring(userId) ?? new List<AnswerForScoringDto>();
            // ممكن تجيب هوية المستخدم من نفس API ثاني، أو تمرّر بالكواري لو عندك
            // هون بنبني ViewModel للعرض والتقييم
            var vm = new ScorePostVm
            {
                UserId = userId,
                Items = answers.Select(a => new ScoreRowVm
                {
                    AnswerId = a.AnswerId,
                    QuestionId = a.QuestionId,
                    QuestionItemId = a.QuestionItemId,
                    ItemTextAr = a.ItemTextAr,
                    ItemTextEn = a.ItemTextEn,
                    Value = a.Value,
                    SubmittedAt = a.SubmittedAt,
                    Score = 0,        // افتراضي
                    Notes = null
                }).ToList()
            };

            return View(vm);
        }

        // POST /AnswerScoring/AddScores
        // يستقبل الفورم من ByUser ويحوّله ل ScorePostDto ثم ينادي API
        [HttpPost("AddScores")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddScores(ScorePostVm vm)
        {
            if (vm == null || vm.Items == null || vm.Items.Count == 0)
            {
                TempData["Err"] = "لا توجد عناصر لإرسالها.";
                return RedirectToAction(nameof(Index));
            }

            // تحويل إلى DTO للإرسال
            var dto = new ScorePostDto
            {
                Items = vm.Items.Select(x => new ScoreItemDto
                {
                    AnswerId = x.AnswerId,
                    Score = x.Score,
                    Notes = x.Notes
                }).ToList()
            };

            await _svc.AddScore(dto);

            TempData["Ok"] = "تم حفظ التقييمات بنجاح.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("AllScores")]
        public async Task<IActionResult> AllScores()
        {
            var data = await _svc.AllScores() ?? new List<AllScoresDto>();
            return View(data);
        }

        [HttpGet("ReviewersSummary")]
        public async Task<IActionResult> ReviewersSummary()
        {
            var data = await _svc.ReviewersSummary() ?? new List<ReviewersSummaryDto>();
            return View(data);
        }

    }
}
