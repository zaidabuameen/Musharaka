using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Musharaka.Models;
using Microsoft.AspNetCore.Authorization;

[Authorize(Roles = "MinistryAdmin")] // فقط الوزارة تدخل هون
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // عرض قائمة المستخدمين
    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        return View(users);
    }

    // تغيير الرتبة (Action)
    [HttpPost]
    public async Task<IActionResult> ChangeRole(string userId, string newRole)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            // حذف الرتب القديمة
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // إضافة الرتبة الجديدة
            if (await _roleManager.RoleExistsAsync(newRole))
            {
                await _userManager.AddToRoleAsync(user, newRole);
            }
        }
        return RedirectToAction(nameof(Index));
    }
}