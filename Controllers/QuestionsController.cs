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

        public QuestionsController(QuestionsService svc, IStringLocalizer<SharedResource> L, LookupsService lookupsService)
        {
            _L = L; _svc = svc;
            _lookupsService = lookupsService;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _svc.List() ?? new();
            return View(list);
        }
        //public IActionResult Create() => View(new QuestionCreateDto());

        [HttpPost]
        public async Task<IActionResult> Create(QuestionCreateDto model)
        {
            await _svc.Create(model);
            TempData["msg"] = SharedResource.Msg_Added;
            return RedirectToAction(nameof(Index));
        }

        //public async Task<IActionResult> Edit(int id)
        //{
        //    var q = await _svc.Get(id); if (q == null) return NotFound();
        //    ViewBag.QuestionId = id;
        //    return View(new QuestionUpdateDto { title = q.TitleEn, description = q.description, categoryId = q.categoryId });
        //}

        //[HttpPost]
        //public async Task<IActionResult> Edit(int id, QuestionUpdateDto model)
        //{
        //    await _svc.Update(id, model);
        //    TempData["msg"] = SharedResource.Msg_Updated;
        //    return RedirectToAction(nameof(Index));
        //}
        //[HttpPost]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    await _svc.Delete(id);
        //    TempData["msg"] = SharedResource.Msg_Deleted;
        //    return RedirectToAction(nameof(Index));
        //}
        [HttpGet]
        public async Task<IActionResult> UpsertPartial(int? id)
        {
            var culture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

            // أنواع السؤال (Multi-Select)
            var types = await _svc.GetTypes(); // يرجع List<(int Id, string Name)>
            ViewBag.QuestionTypes = new SelectList((System.Collections.IEnumerable)types, "Id", "Name");

            var Category = await _lookupsService.List();
            ViewBag.Categorys = new SelectList(Category, "Id", culture == "ar" ? "NameAr" : "NameEn");

            var QuestionItems = await _svc.ListQuestionItems();
            ViewBag.QuestionItems = new SelectList(QuestionItems, "Id", culture == "ar" ? "TextAr" : "TextEn");




            // تحضير الموديل
            var model = new QuestionCreateDto();

            if (id.HasValue)
            {
                // جلب السؤال للتعديل
                var existing = await _svc.Get(id.Value);
                if (existing == null) return NotFound();

                model.TitleEn = existing.TitleEn;
                model.DescriptionEn = existing.DescriptionEn;
                model.CategoryId = existing.CategoryId;
                // items: ما منرجّعها هون (بس نهيئها للتحديث لاحقاً)
                ViewBag.Id = id.Value;
            }

            return PartialView("_QuestionPopup", model);
        }
    }
}
