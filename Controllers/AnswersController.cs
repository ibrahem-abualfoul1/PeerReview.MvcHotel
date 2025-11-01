using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using PeerReview.MvcHotel.Models;
using PeerReview.MvcHotel.Resources;
using PeerReview.MvcHotel.Services;

namespace PeerReview.MvcHotel.Controllers
{
    public class AnswersController : Controller
    {
        private readonly AnswersService _svc;
        private readonly QuestionsService _questions;
        private readonly IStringLocalizer<SharedResource> _L;

        public AnswersController(AnswersService svc, IStringLocalizer<SharedResource> L, QuestionsService questions)
        {
            _L = L;
            _svc = svc; _questions = questions;
        }

        public async Task<IActionResult> Mine() { var list = await _svc.Mine() ?? new(); return View(list); }
        public IActionResult Create() => View(new AnswerCreateDto());
        [HttpPost] public async Task<IActionResult> Create(AnswerCreateDto dto)
        {
            await _svc.Create(dto);
            TempData["msg"] = _L["Msg_Added"]; 
            return RedirectToAction(nameof(Mine));
        }
        public IActionResult Edit(int id) => View(new AnswerUpdateDto());
        [HttpPost] public async Task<IActionResult> Edit(int id, AnswerUpdateDto dto) {
            await _svc.Update(id, dto);
            TempData["msg"] = _L["Msg_Updated"];
            return RedirectToAction(nameof(Mine));
        }
        [HttpPost] public async Task<IActionResult> Delete(int id) {
            await _svc.Delete(id);
            TempData["msg"] = _L["Msg_Deleted"];
            return RedirectToAction(nameof(Mine));
        }

        [HttpPost]
        public async Task<IActionResult> Upload(int questionId, int questionItemId, IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            await _svc.Upload(questionId, questionItemId, file.FileName, ms.ToArray(), file.ContentType);
            TempData["msg"] = _L["Msg_Uploaded"]; return RedirectToAction(nameof(Mine));
        }


        //[HttpGet("/Answers/Screen/{id:int}")]
        //public async Task<IActionResult> Screen(int id, CancellationToken ct)
        //{
        //    var q = await _questions.Get(id, ct);
        //    ViewBag.QuestionId = id;
        //    return View(q);
        //}

        [HttpPost]
        public async Task<IActionResult> SaveSet([FromBody] AnswerSetPost body, CancellationToken ct)
        {
            if (body == null || body.Values == null) return BadRequest();
            await _svc.BulkCreate(body.QuestionId, body.Values, ct);
            TempData["msg"] = _L["Msg_Added"];
            return Ok();
        }
    }
}