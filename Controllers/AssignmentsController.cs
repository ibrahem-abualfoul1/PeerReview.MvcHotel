using Microsoft.Extensions.Localization;

using Microsoft.AspNetCore.Mvc;
using PeerReview.MvcHotel.Models;
using PeerReview.MvcHotel.Services;

namespace PeerReview.MvcHotel.Controllers
{
    public class AssignmentsController : Controller
    {
        private readonly AssignmentsService _svc;
        private readonly UsersService _users;
        private readonly QuestionsService _questions;
        private readonly IStringLocalizer<PeerReview.MvcHotel.Models.SharedResource> _L;

        public AssignmentsController(AssignmentsService svc, UsersService users, QuestionsService questions, IStringLocalizer<PeerReview.MvcHotel.Models.SharedResource> L){
            _L = L; _svc=svc; _users=users; _questions=questions; }

        public async Task<IActionResult> Index(int? userId, int? questionId)
        {
            var list = new List<AssignmentDto>();
            if (userId.HasValue) list = await _svc.ByUser(userId.Value) ?? new();
            else if (questionId.HasValue) list = await _svc.ByQuestion(questionId.Value) ?? new();
            ViewBag.Users = await _users.List() ?? new();
            ViewBag.Questions = await _questions.List() ?? new();
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> Bulk([FromBody] AssignRequest req){ await _svc.BulkAssign(req); return Ok(); }
        [HttpPost] public async Task<IActionResult> Activate(int id){ await _svc.Activate(id); TempData["msg"]=_L["Msg_Activated"]; return RedirectToAction(nameof(Index)); }
        [HttpPost] public async Task<IActionResult> Deactivate(int id){ await _svc.Deactivate(id); TempData["msg"]=_L["Msg_Deactivated"]; return RedirectToAction(nameof(Index)); }
    }
}
