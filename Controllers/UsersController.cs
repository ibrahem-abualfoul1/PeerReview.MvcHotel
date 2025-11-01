using Microsoft.Extensions.Localization;

using Microsoft.AspNetCore.Mvc;
using PeerReview.MvcHotel.Models;
using PeerReview.MvcHotel.Services;
using PeerReview.MvcHotel.Resources;

namespace PeerReview.MvcHotel.Controllers
{
    public class UsersController : Controller
    {
        private readonly UsersService _svc;
        private readonly IStringLocalizer<SharedResource> _L;

        public UsersController(UsersService svc, IStringLocalizer<SharedResource> L){
            _L = L; _svc = svc; }

        public async Task<IActionResult> Index(){ var list = await _svc.List() ?? new(); return View(list); }
        public IActionResult Create() => View(new UserCreateDto());

        [HttpPost] public async Task<IActionResult> Create(UserCreateDto model){ if(!ModelState.IsValid) return View(model); await _svc.Create(model); TempData["msg"]=SharedResource.Msg_Added; return RedirectToAction(nameof(Index)); }

        public async Task<IActionResult> Edit(int id){
            var u = await _svc.Get(id);
            if (u==null) return NotFound();
            ViewBag.UserId = id; ViewBag.UserName = u.userName;
            return View(new UserUpdateDto{ fullName=u.fullName, email=u.email, isActive=u.isActive, roleId=u.roleId });
        }

        [HttpPost] public async Task<IActionResult> Edit(int id, UserUpdateDto model){ 
            await _svc.Update(id, model);
            TempData["msg"]= SharedResource.Msg_Added;
            return RedirectToAction(nameof(Index)); }
        [HttpPost] public async Task<IActionResult> Delete(int id){
            await _svc.Delete(id); 
            TempData["msg"]=SharedResource.Msg_Added;
            return RedirectToAction(nameof(Index));
        }
        [HttpPost] public async Task<IActionResult> Activate(int id){
            await _svc.Activate(id);
            return RedirectToAction(nameof(Index)); }
        [HttpPost] public async Task<IActionResult> Deactivate(int id){
            await _svc.Deactivate(id);
            return RedirectToAction(nameof(Index)); }
    }
}
