using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Mvc;
using PeerReview.MvcHotel.Models;
using PeerReview.MvcHotel.Services;
using PeerReview.MvcHotel.Resources;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PeerReview.MvcHotel.Controllers
{
    public class AssignmentsController : Controller
    {
        private readonly AssignmentsService _svc;
        private readonly UsersService _users;
        private readonly QuestionsService _questions;
        private readonly IStringLocalizer<SharedResource> _L;

        public AssignmentsController(
            AssignmentsService svc,
            UsersService users,
            QuestionsService questions,
            IStringLocalizer<SharedResource> L)
        {
            _L = L; _svc = svc; _users = users; _questions = questions;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? userId, int? questionId)
        {
            var list = new List<AssignmentDto>();
            if (userId.HasValue) list = await _svc.ByUser(userId.Value) ?? new();
            else if (questionId.HasValue) list = await _svc.ByQuestion(questionId.Value) ?? new();

            ViewBag.Users = await _users.List() ?? new();
            ViewBag.Questions = await _questions.List() ?? new();

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Table(int? userId, int? questionId)
        {
            var list = new List<AssignmentDto>();
            if (userId.HasValue) list = await _svc.ByUser(userId.Value) ?? new();
            else if (questionId.HasValue) list = await _svc.ByQuestion(questionId.Value) ?? new();

            ViewBag.Users = await _users.List() ?? new();
            ViewBag.Questions = await _questions.List() ?? new();

            return PartialView("_AssignmentsTable", list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            await _svc.Activate(id);
            return Json(new { ok = true, message = _L[SharedResource.Msg_Activated] });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _svc.Deactivate(id);
            return Json(new { ok = true, message = _L[SharedResource.Msg_Deactivated] });
        }

        [HttpGet]
        public async Task<IActionResult> Upsert()
        {
            var users = await _users.List() ?? new();
            var qs = await _questions.List() ?? new();
            ViewBag.UsersSelect = new SelectList(users, "id", "fullName");
            ViewBag.QuestionsSelect = new SelectList(qs, "id", "title");
            return PartialView("_AssignmentUpsertModal"); 
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert([FromBody] AssignRequest req)
        {
            
            await _svc.BulkAssign(req);
            return Json(new { ok = true, message = _L[SharedResource.Msg_Welcome] });
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Bulk([FromBody] AssignRequest req)
        {
            await _svc.BulkAssign(req);
            return Json(new { ok = true, message = _L[SharedResource.Msg_Welcome] });
        }
    }
}
