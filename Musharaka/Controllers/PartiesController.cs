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

        // الـ Constructor لازم يستلم الاثنين مع بعض
        public PartiesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // الـ Index الوحيدة اللي لازم تضل
        public async Task<IActionResult> Index()
        {
            // جلب الأحزاب
            var parties = await _context.PoliticalParties.ToListAsync();

            // جلب المستخدمين للجدول الثاني
            ViewBag.AllUsers = await _userManager.Users.ToListAsync();

            return View(parties);
        }
        //// GET: Parties
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.PoliticalParties.ToListAsync());
        //}

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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PartyId,Name,Description,Ideology,FoundDate,LogoUrl,Status,City,Address")] PoliticalParty politicalParty)
        {
            if (ModelState.IsValid)
            {
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
            return View(politicalParty);
        }

        // POST: Parties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PartyId,Name,Description,Ideology,FoundDate,LogoUrl,Status,City,Address")] PoliticalParty politicalParty)
        {
            if (id != politicalParty.PartyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
            // جلب الأحزاب من قاعدة البيانات
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

            return Ok(parties); // برجع البيانات كـ JSON مع كود 200 (Success)
            //http://localhost:5296/api/PartiesApi


        }

    }

}
