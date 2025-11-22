using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using PeerReview.MvcHotel.Models;
using PeerReview.MvcHotel.Resources;
using PeerReview.MvcHotel.Services;

namespace PeerReview.MvcHotel.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly QuestionsService _svc;
        private readonly IStringLocalizer<SharedResource> _L;
        private readonly LookupsService _lookupsService;

        public QuestionsController(
            QuestionsService svc,
            IStringLocalizer<SharedResource> L,
            LookupsService lookupsService)
        {
            _L = L;
            _svc = svc;
            _lookupsService = lookupsService;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _svc.List() ?? new();
            return View(list);
        }

        //[HttpGet]
        //public IActionResult Create() => View(new QuestionCreateDto());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestionCreateDto model)
        {
            await _svc.Create(model);
            TempData["msg"] = _L["Msg_Added"].Value;
            return RedirectToAction(nameof(Index));
        }

        // مثال لو رجعت تفعّل Edit لاحقاً:
        //
        //[HttpGet]
        //public async Task<IActionResult> Edit(int id)
        //{
        //    var q = await _svc.Get(id);
        //    if (q == null) return NotFound();
        //
        //    ViewBag.QuestionId = id;
        //    return View(new QuestionUpdateDto
        //    {
        //        TitleEn = q.TitleEn,
        //        DescriptionEn = q.DescriptionEn,
        //        CategoryId = q.CategoryId
        //    });
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, QuestionUpdateDto model)
        //{
        //    await _svc.Update(id, model);
        //    TempData["msg"] = _L["Msg_Updated"];
        //    return RedirectToAction(nameof(Index));
        //}

        [HttpGet]
        public async Task<IActionResult> UpsertPartial(int? id)
        {
            var culture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

            // أنواع السؤال (Multi-Select)
            var types = await _svc.GetTypes() ?? new List<QuestionTypeDto>();
            ViewBag.QuestionTypes = new SelectList(
                (System.Collections.IEnumerable)types,
                "Id",
                "Name"
            );

            var category = await _lookupsService.List();
            ViewBag.Categorys = new SelectList(
                category,
                "Id",
                culture == "ar" ? "NameAr" : "NameEn"
            );

            var questionItems = await _svc.ListQuestionItems();
            
            ViewBag.QuestionItems = questionItems;

            // تحضير الموديل
            var model = new QuestionCreateDto();

            if (id.HasValue)
            {
                // جلب السؤال للتعديل
                var existing = await _svc.Get(id.Value);
                if (existing == null) return NotFound();

                model.TitleEn = existing.TitleEn;
                model.TitleAr = existing.TitleAr;
                model.DescriptionEn = existing.DescriptionEn;
                model.DescriptionAr = existing.DescriptionAr;
                model.CategoryId = existing.CategoryId;
                model.Items = existing.Items?.Select(item => item.Id).ToList();

                ViewBag.Id = id.Value;
            }

            return PartialView("_QuestionPopup", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _svc.Delete(id);
            TempData["msg"] = _L["Msg_Deleted"].Value;
            return RedirectToAction(nameof(Index));
        }
    }
}
