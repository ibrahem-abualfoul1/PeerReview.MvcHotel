using Microsoft.Extensions.Localization;

using Microsoft.AspNetCore.Mvc;
using PeerReview.MvcHotel.Models;
using PeerReview.MvcHotel.Services;

namespace PeerReview.MvcHotel.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly QuestionsService _svc;
        private readonly IStringLocalizer<PeerReview.MvcHotel.Models.SharedResource> _L;

        public QuestionsController(QuestionsService svc, IStringLocalizer<PeerReview.MvcHotel.Models.SharedResource> L){
            _L = L; _svc = svc; }

        public async Task<IActionResult> Index(){ var list = await _svc.List() ?? new(); return View(list); }
        public IActionResult Create() => View(new QuestionCreateDto());

        [HttpPost] public async Task<IActionResult> Create(QuestionCreateDto model){ await _svc.Create(model); TempData["msg"]=_L["Msg_Added"]; return RedirectToAction(nameof(Index)); }

        public async Task<IActionResult> Edit(int id){
            var q = await _svc.Get(id); if (q==null) return NotFound();
            ViewBag.QuestionId = id;
            return View(new QuestionUpdateDto{ title=q.title, description=q.description, categoryId=q.categoryId });
        }

        [HttpPost] public async Task<IActionResult> Edit(int id, QuestionUpdateDto model){ await _svc.Update(id, model); TempData["msg"]=_L["Msg_Updated"]; return RedirectToAction(nameof(Index)); }
        [HttpPost] public async Task<IActionResult> Delete(int id){ await _svc.Delete(id); TempData["msg"]=_L["Msg_Deleted"]; return RedirectToAction(nameof(Index)); }
    }
}
