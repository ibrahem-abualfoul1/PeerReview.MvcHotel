using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using PeerReview.MvcHotel.Models;
using PeerReview.MvcHotel.Resources;
using PeerReview.MvcHotel.Services;

namespace PeerReview.MvcHotel.Controllers
{
    [Route("[controller]")] 
    public class LookupsController : Controller
    {
        private readonly LookupsService _svc;
        private readonly IStringLocalizer<SharedResource> _L;

        public LookupsController(LookupsService svc, IStringLocalizer<SharedResource> L)
        {
            _svc = svc;
            _L = L;
        }

        // --------------------------------------------------
        // LOOKUPS
        // --------------------------------------------------

        [HttpGet("")] // GET /Lookups
        public async Task<IActionResult> Index()
        {
            var list = await _svc.List() ?? new();
            return View(list);
        }

        [HttpGet("Create")] // GET /Lookups/Create
        public IActionResult Create() => View();

        [HttpPost("Create")] // POST /Lookups/Create
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LookupCreateDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _svc.Create(dto);
            TempData["msg"] = _L["Msg_Added"].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Edit/{code}")] // GET /Lookups/Edit/{code}
        public async Task<IActionResult> Edit(string code)
        {
            var lookup = await _svc.Get(code);
            if (lookup == null) return NotFound();

            var dto = new LookupUpdateDto
            {
                Code = lookup.Code,
                NameEn = lookup.NameEn,
                NameAr = lookup.NameAr,
                TypeEn = lookup.TypeEn,
                TypeAr = lookup.TypeAr
            };
            return View(dto);
        }

        [HttpPost("Edit/{code}")] // POST /Lookups/Edit/{code}
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string code, LookupUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _svc.Update(code, dto);
            TempData["msg"] = _L["Msg_Updated"].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Delete/{code}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string code)
        {
            await _svc.Delete(code);
            TempData["msg"] = _L["Msg_Deleted"].Value;
            return RedirectToAction(nameof(Index));
        }

        // --------------------------------------------------
        // SUB LOOKUPS
        // --------------------------------------------------

        [HttpGet("Sub/{id:int}")] // GET /Lookups/Sub/{id}
        public async Task<IActionResult> Sub(int id)
        {
            var list = await _svc.Sub(id) ?? new();
            ViewBag.LookupId = id;
            return View(list);
        }

        [HttpGet("AddSub/{id}")] // GET /Lookups/AddSub/{code}
        public IActionResult AddSub(int id)
        {
            ViewBag.Code = id;
            return View();
        }

        [HttpPost("AddSub/{id}")] // POST /Lookups/AddSub/{code}
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSub(int id, SubLookupCreateDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _svc.AddSub(id, dto);
            TempData["msg"] = _L["Msg_Added"].Value;
            return RedirectToAction("Sub", new { id });
        }

        [HttpGet("EditSub/{id:int}")] // GET /Lookups/EditSub/{id}
        public async Task<IActionResult> EditSub(int id)
        {
            // ملاحظة مهمة: استدعاء _svc.Sub(id) يرجّع قائمة بناءً على lookupId عادةً،
            // فهذا غالبًا خطأ. الأفضل تعمل خدمة تجيب Sub واحد بالـ Id.
            var sub = await _svc.GetSubById(id); // <-- أنشئ/استخدم هذه الخدمة
            if (sub == null) return NotFound();
            return View(sub);
        }

        [HttpPost("EditSub/{id:int}")] // POST /Lookups/EditSub/{id}
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSub(int id, SubLookupUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _svc.UpdateSub(id, dto);
            TempData["msg"] = _L["Msg_Updated"].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("DeleteSub/{id:int}")] // POST /Lookups/DeleteSub/{id}
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSub(int id)
        {
            await _svc.DeleteSub(id);
            TempData["msg"] = _L["Msg_Deleted"].Value;
            return RedirectToAction(nameof(Index));
        }
    }
}
