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
    [Authorize(Roles = "MinistryAdmin,PartyAdmin")]
    public class MembershipsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // تم إضافة UserManager هنا لتمكين الفلترة بناءً على المستخدم المسجل
        public MembershipsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Memberships
        // تم تعديل الميثود لتطبيق منطق السيناريو (الوزارة مقابل الحزب)
        public async Task<IActionResult> Index()
        {
            // 1. جلب الكويرة الأساسية مع الربط بالجداول الأخرى
            var membershipsQuery = _context.Memberships
                .Include(m => m.Party)
                .Include(m => m.User)
                .AsQueryable();

            // 2. إذا كان المستخدم "أدمن حزب" وليس "وزارة"، نقوم بفلترة الطلبات الخاصة بحزبه فقط
            if (!User.IsInRole("MinistryAdmin") && User.IsInRole("PartyAdmin"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                // الفلترة تتم بناءً على أن الحزب مرتبط بهذا الأدمن (AdminId)
                membershipsQuery = membershipsQuery.Where(m => m.Party.AdminId == currentUser.Id);
            }

            return View(await membershipsQuery.ToListAsync());
        }

        // GET: Memberships/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membership = await _context.Memberships
                .Include(m => m.Party)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.MembershipId == id);

            if (membership == null)
            {
                return NotFound();
            }

            return View(membership);
        }

        // GET: Memberships/Create
        public IActionResult Create()
        {
            ViewData["PartyId"] = new SelectList(_context.PoliticalParties, "PartyId", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email"); // تم التغيير لعرض الإيميل بدل الـ Id
            return View();
        }

        // POST: Memberships/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MembershipId,UserId,PartyId,NationalIdCardUrl,JobTitle,EducationLevel,JoinDate,MembershipStatus,AdminNotes")] Membership membership)
        {
            if (ModelState.IsValid)
            {
                _context.Add(membership);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PartyId"] = new SelectList(_context.PoliticalParties, "PartyId", "Name", membership.PartyId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", membership.UserId);
            return View(membership);
        }

        // GET: Memberships/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membership = await _context.Memberships.FindAsync(id);
            if (membership == null)
            {
                return NotFound();
            }
            ViewData["PartyId"] = new SelectList(_context.PoliticalParties, "PartyId", "Name", membership.PartyId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", membership.UserId);
            return View(membership);
        }

        // POST: Memberships/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MembershipId,UserId,PartyId,NationalIdCardUrl,JobTitle,EducationLevel,JoinDate,MembershipStatus,AdminNotes")] Membership membership)
        {
            if (id != membership.MembershipId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(membership);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MembershipExists(membership.MembershipId))
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
            ViewData["PartyId"] = new SelectList(_context.PoliticalParties, "PartyId", "Name", membership.PartyId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", membership.UserId);
            return View(membership);
        }

        // GET: Memberships/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membership = await _context.Memberships
                .Include(m => m.Party)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.MembershipId == id);
            if (membership == null)
            {
                return NotFound();
            }

            return View(membership);
        }

        // POST: Memberships/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var membership = await _context.Memberships.FindAsync(id);
            if (membership != null)
            {
                _context.Memberships.Remove(membership);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MembershipExists(int id)
        {
            return _context.Memberships.Any(e => e.MembershipId == id);
        }
    }
}