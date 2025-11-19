using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using PeerReview.MvcHotel.Models;
using PeerReview.MvcHotel.Resources;
using PeerReview.MvcHotel.Services;

namespace PeerReview.MvcHotel.Controllers
{
    public class UsersController : Controller
    {
        private readonly UsersService _svc;
        private readonly IStringLocalizer<SharedResource> _L;
        private readonly RolesService _svcRole;

        public UsersController(
            UsersService svc,
            IStringLocalizer<SharedResource> L,
            RolesService svcRole)
        {
            _L = L;
            _svc = svc;
            _svcRole = svcRole;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _svc.List() ?? new();
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            var list = await _svcRole.List() ?? new();

            ViewBag.Roles = list
                .Select(r => new SelectListItem
                {
                    Value = r.id.ToString(),
                    Text = r.name
                })
                .ToList();

            return View(new UserCreateDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _svc.Create(model);
            TempData["msg"] = _L["Msg_Added"];
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var u = await _svc.Get(id);
            if (u == null) return NotFound();

            var roles = await _svcRole.List() ?? new();
            var roleItems = roles
                .Select(r => new SelectListItem
                {
                    Value = r.id.ToString(),
                    Text = r.name ?? $"Role #{r.id}",
                    Selected = (r.id == u.roleId)
                })
                .ToList();

            ViewBag.Roles = roleItems;
            ViewBag.UserId = id;
            ViewBag.UserName = u.userName;

            var vm = new UserUpdateDto
            {
                fullName = u.fullName,
                email = u.email,
                isActive = u.isActive,
                roleId = u.roleId
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserUpdateDto model)
        {
            await _svc.Update(id, model);
            TempData["msg"] = _L["Msg_Updated"];
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _svc.Delete(id);
            TempData["msg"] = _L["Msg_Deleted"];
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            await _svc.Activate(id);
            // ممكن تضيف:
            // TempData["msg"] = _L["Msg_Activated"];
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _svc.Deactivate(id);
            // ممكن تضيف:
            // TempData["msg"] = _L["Msg_Deactivated"];
            return RedirectToAction(nameof(Index));
        }
    }
}
