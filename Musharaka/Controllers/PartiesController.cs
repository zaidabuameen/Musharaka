using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Musharaka.Data;
using Musharaka.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Musharaka.Controllers
{
    [Authorize(Roles = "MinistryAdmin")]
    public class PartiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PartiesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Parties
        // تم التحديث ليشمل جلب طلبات الانتساب (Memberships) وحساب الإحصائيات للداشبورد
        public async Task<IActionResult> Index()
        {
            // 1. جلب الأحزاب مع تضمين الـ Memberships لحساب عدد الطلبات المعلقة لكل حزب ديناميكياً
            var parties = await _context.PoliticalParties
                .Include(p => p.Memberships)
                .ToListAsync();

            // 2. جلب المستخدمين للجدول الثاني (إدارة الصلاحيات المدمج)
            ViewBag.AllUsers = await _userManager.Users.ToListAsync();

            // 3. حساب إحصائيات الـ Dashboard العلوي وتمريرها للصفحة
            ViewBag.TotalParties = parties.Count;
            ViewBag.ActiveParties = parties.Count(p => p.Status == "Active");
            ViewBag.PendingParties = parties.Count(p => p.Status == "Under Review" || p.Status == "Pending");
            ViewBag.TotalUsers = await _userManager.Users.CountAsync();

            return View(parties);
        }

        // GET: Parties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var politicalParty = await _context.PoliticalParties
                .FirstOrDefaultAsync(m => m.PartyId == id);
            if (politicalParty == null)
            {
                return NotFound();
            }

            return View(politicalParty);
        }

        // GET: Parties/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Parties/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PartyId,Name,Description,Ideology,FoundDate,LogoUrl,Status,City,Address")] PoliticalParty politicalParty)
        {
            if (ModelState.IsValid)
            {
                // بشكل افتراضي عند الإنشاء نربطه بالمستخدم الحالي ونضع الحالة قيد المراجعة
                politicalParty.AdminId = _userManager.GetUserId(User);
                politicalParty.Status = "Under Review";

                _context.Add(politicalParty);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(politicalParty);
        }

        // GET: Parties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var politicalParty = await _context.PoliticalParties.FindAsync(id);
            if (politicalParty == null)
            {
                return NotFound();
            }

            // جلب المستخدمين لتعبئة القائمة المنسدلة لاختيار رئيس الحزب عند التعديل
            var users = await _userManager.Users.Select(u => new {
                u.Id,
                FullName = u.FirstName + " " + u.LastName + " (" + u.Email + ")"
            }).ToListAsync();

            ViewData["AdminId"] = new SelectList(users, "Id", "FullName", politicalParty.AdminId);

            return View(politicalParty);
        }

        // POST: Parties/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PartyId,Name,Description,Ideology,FoundDate,LogoUrl,Status,City,Address,AdminId")] PoliticalParty politicalParty)
        {
            if (id != politicalParty.PartyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // جلب البيانات الأصلية قبل الحفظ للتأكد من حالة الحزب القديمة
                    var oldData = await _context.PoliticalParties.AsNoTracking().FirstOrDefaultAsync(p => p.PartyId == id);

                    // الأتمتة: إذا وافقت الوزارة وتحولت الحالة إلى Active، يتم ترقية صاحب الحزب إلى PartyAdmin فوراً
                    if (politicalParty.Status == "Active" && oldData?.Status != "Active")
                    {
                        if (!string.IsNullOrEmpty(politicalParty.AdminId))
                        {
                            var user = await _userManager.FindByIdAsync(politicalParty.AdminId);
                            if (user != null)
                            {
                                await _userManager.AddToRoleAsync(user, "PartyAdmin");
                            }
                        }
                    }

                    _context.Update(politicalParty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PoliticalPartyExists(politicalParty.PartyId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // إعادة تعبئة القائمة المنسدلة في حال فشل الـ Validation
            var users = await _userManager.Users.Select(u => new {
                u.Id,
                FullName = u.FirstName + " " + u.LastName + " (" + u.Email + ")"
            }).ToListAsync();
            ViewData["AdminId"] = new SelectList(users, "Id", "FullName", politicalParty.AdminId);

            return View(politicalParty);
        }

        // GET: Parties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var politicalParty = await _context.PoliticalParties
                .FirstOrDefaultAsync(m => m.PartyId == id);
            if (politicalParty == null)
            {
                return NotFound();
            }

            return View(politicalParty);
        }

        // POST: Parties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var politicalParty = await _context.PoliticalParties.FindAsync(id);
            if (politicalParty != null)
            {
                _context.PoliticalParties.Remove(politicalParty);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PoliticalPartyExists(int id)
        {
            return _context.PoliticalParties.Any(e => e.PartyId == id);
        }

        // GET: api/PartiesApi
        [HttpGet("api/PartiesApi")]
        public async Task<IActionResult> GetPartiesApi()
        {
            // جلب الأحزاب من قاعدة البيانات كـ JSON للـ Flutter
            var parties = await _context.PoliticalParties
                .Select(p => new {
                    p.PartyId,
                    p.Name,
                    p.Description,
                    p.Ideology,
                    p.LogoUrl,
                    p.City,
                    p.Status
                })
                .ToListAsync();

            return Ok(parties);
        }
    }
}