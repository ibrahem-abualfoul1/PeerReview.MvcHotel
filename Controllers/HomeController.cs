
using Microsoft.AspNetCore.Mvc;
using PeerReview.MvcHotel.Services;

namespace PeerReview.MvcHotel.Controllers
{
    public class HomeController : Controller
    {
        private readonly UsersService _users;
        private readonly QuestionsService _questions;

        public HomeController(UsersService users, QuestionsService questions){ _users = users; _questions = questions; }

        public async Task<IActionResult> Index()
        {
            try{
                var users = await _users.List() ?? new();
                var qs = await _questions.List() ?? new();
                ViewBag.UsersCount = users.Count;
                ViewBag.QuestionsCount = qs.Count;
            } catch { ViewBag.UsersCount = 0; ViewBag.QuestionsCount = 0; }
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
