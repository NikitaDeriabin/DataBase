using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryWebApplication;

namespace LibraryWebApplication.Controllers
{
    public class FilialsController : Controller
    {
        private readonly DBLibraryContext _context;

        public FilialsController(DBLibraryContext context)
        {
            _context = context;
        }

        // GET: Filials
        public async Task<IActionResult> Index(int? id, string? name)
        {
            if (id == null) return RedirectToAction("Index", "Companies");
            //Знаходження філіалів за компанією
            ViewBag.CompanyId = id;
            ViewBag.CompanyName = name;
            var filialsByCompany = _context.Filials.Where(b => b.CompanyId == id).Include(b => b.Company).Include(f => f.Country);
            return View(await filialsByCompany.ToListAsync());

            //var dBLibraryContext = _context.Filials.Include(f => f.Company).Include(f => f.Country);
            //return View(await dBLibraryContext.ToListAsync());
        }

        // GET: Filials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filials = await _context.Filials
                .Include(f => f.Company)
                .Include(f => f.Country)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (filials == null)
            {
                return NotFound();
            }

            return View(filials);
        }

        // GET: Filials/Create
        public IActionResult Create(int companyId)
        {
            // ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Id");
            // return View();

            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name");
            ViewBag.CompanyId = companyId;
            ViewBag.CompanyName = _context.Companies.Where(c => c.Id == companyId).FirstOrDefault().Name;
            return View();
        }

        // POST: Filials/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int companyId, [Bind("Id,Name,Amount,CompanyId,CountryId")] Filials filial)
        {

            filial.CompanyId = companyId;
            if (ModelState.IsValid)
            {
                _context.Add(filial);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Index", "Filials", new { id = companyId, name = _context.Companies.Where(c => c.Id == companyId).FirstOrDefault().Name });
            }
            //ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Id", filials.CompanyId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Id", filial.CountryId);
            //return View(filials);
            return RedirectToAction("Index", "Filials", new { id = companyId, name = _context.Companies.Where(c => c.Id == companyId).FirstOrDefault().Name });
        }

        // GET: Filials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
           
            if (id == null)
            {
                return NotFound();
            }

            var filials = await _context.Filials.FindAsync(id);
            if (filials == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", filials.CompanyId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", filials.CountryId);
            return View(filials);
        }

        // POST: Filials/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Amount,CompanyId,CountryId")] Filials filials)
        {
            int? companyId = filials.CompanyId;
            string? companyName = _context.Companies.Where(c => c.Id == companyId).FirstOrDefault().Name;
            if (id != filials.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(filials);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilialsExists(filials.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Filials", new { id = companyId, name = companyName });
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Id", filials.CompanyId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Id", filials.CountryId);
            return RedirectToAction("Index", "Filials", new { id = companyId, name = companyName });

            //return View(filials);

        }

        // GET: Filials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filials = await _context.Filials
                .Include(f => f.Company)
                .Include(f => f.Country)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (filials == null)
            {
                return NotFound();
            }

            return View(filials);
        }

        // POST: Filials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var filials = await _context.Filials.FindAsync(id);
            _context.Filials.Remove(filials);
            await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
            return RedirectToAction("Index", "Filials", new { id = filials.CompanyId, name = _context.Companies.Where(c => c.Id == filials.CompanyId).FirstOrDefault().Name });
        }

        private bool FilialsExists(int id)
        {
            return _context.Filials.Any(e => e.Id == id);
        }
    }
}
