using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PeerReview.MvcHotel.Models;
using PeerReview.MvcHotel.Services;

namespace PeerReview.MvcHotel.Controllers
{
    [Route("user-questions")]
    public class UserQuestionsController : Controller
    {
        private readonly AnswersService _answersService;
        private readonly IQuestionsService _questions;
        private readonly IHttpContextAccessor _http;
        private readonly AssignmentsService _assignmentsService;

        public UserQuestionsController(IQuestionsService questions ,AnswersService answersService, IHttpContextAccessor http, AssignmentsService assignmentsService)
        {
            _answersService = answersService;
            _http = http;
            _assignmentsService = assignmentsService;
            _questions = questions;
        }

        [HttpGet("survey/{userId:int}")]
        public async Task<IActionResult> Survey(int userId)
        {
            List<AssignmentDto> vm =  await _assignmentsService.ByUser(userId);

            SurveyViewModel surveyVm = new SurveyViewModel();

            foreach (var item in vm)
            {
                surveyVm.Groups.Add(new QuestionGroup
                {
                    Id = item.question.Id,
                    Title = LocalizationExtensions.Localize(
                        item.question.TitleAr,
                        item.question.TitleEn,
                        item.question.TitleEn ?? item.question.TitleAr ?? string.Empty),
                    Description = LocalizationExtensions.Localize(
                        item.question.DescriptionAr,
                        item.question.DescriptionEn),
                    CategoryId = item.question.CategoryId ?? 0,
                    Items = item.question.Items?.Select(qi => new QuestionItem
                    {
                        Id = qi.Id,
                        Text = LocalizationExtensions.Localize(qi.TextAr, qi.TextEn),
                        Type = qi.Type,
                        IsRequired = qi.IsRequired,
                        OptionsCsv = string.Join(',', LocalizationExtensions.LocalizeOptions(qi.OptionsCsvAr, qi.OptionsCsvEn))
                    }).ToList() ?? new List<QuestionItem>()
                });
            }
            

            return View("Survey", surveyVm); // Views/UserQuestions/My.cshtml
        }

        [HttpPost("submit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit([FromBody] List<AnswerCreateDto> answers)
        {
            if (answers is null || answers.Count == 0)
                return BadRequest("No answers received.");

            var userId = _http.HttpContext!.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? "anonymous";

            await _answersService.Create(answers);


            return Ok(new { ok = true, saved = answers.Count });
        }


    }
}
