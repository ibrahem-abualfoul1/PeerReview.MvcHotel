
using Microsoft.AspNetCore.Mvc;
using PeerReview.MvcHotel.Services;

namespace PeerReview.MvcHotel.Controllers
{
    public class HomeController : Controller
    {
        private readonly UsersService _users;
        private readonly QuestionsService _questions;
        private readonly AuthService _authService;

        public HomeController(UsersService users, QuestionsService questions, AuthService authService)
        {
            _users = users; _questions = questions;
            _authService = authService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var users = await _authService.List() ?? new();
                ViewBag.UsersCount = users.Metrics.TotalUsers;
                ViewBag.QuestionsCount            = users.Metrics.TotalQuestions;
                ViewBag.AnswersCount              = users.Metrics.TotalAnswers;
                ViewBag.AnsweredByMeCount         = users.Metrics.AnsweredByMe;
                ViewBag.AssignedToMeCount         = users.Metrics.AssignedToMe;
                ViewBag.MyPendingCount            = users.Metrics.MyPending;
                ViewBag.TotalAssignmentsCount     = users.Metrics.TotalAssignments;


            }
            catch
            {
                ViewBag.QuestionsCount           = 0;
                ViewBag.AnswersCount             = 0;
                ViewBag.AnsweredByMeCount        = 0;
                ViewBag.AssignedToMeCount        = 0;
                ViewBag.MyPendingCount           = 0;
                ViewBag.TotalAssignmentsCount = 0;
            }

            return View();
        }
        public IActionResult GetData()
        {
            List<string> myStringList = new List<string>();
            myStringList.Add("First string");
            myStringList.Add("Second string");
            myStringList.Add("Third string");
            return Ok(myStringList);
        }
    }
}
