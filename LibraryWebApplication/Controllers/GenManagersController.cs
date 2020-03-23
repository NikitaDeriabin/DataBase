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
    public class GenManagersController : Controller
    {
        private readonly DBLibraryContext _context;

        public GenManagersController(DBLibraryContext context)
        {
            _context = context;
        }

        // GET: GenManagers
        public async Task<IActionResult> Index(string searchString = "")
        {
            ViewData["currentFilter"] = searchString;

            if(!String.IsNullOrEmpty(searchString))
            {
                var genManagers = from g in _context.GenManagers
                                  select g;
                genManagers = genManagers.Where(g => g.Name.Contains(searchString) || g.Country.Name.Contains(searchString)).Include(g => g.Country);
                return View(await genManagers.ToListAsync());
            }
            var dBLibraryContext = _context.GenManagers.Include(g => g.Country);
            return View(await dBLibraryContext.ToListAsync());
        }

        // GET: GenManagers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genManagers = await _context.GenManagers
                .Include(g => g.Country)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (genManagers == null)
            {
                return NotFound();
            }

            return View(genManagers);
        }

        // GET: GenManagers/Create
        public IActionResult Create()
        {
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name");
            return View();
        }

        // POST: GenManagers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,YearBirth,Information,CountryId")] GenManagers genManagers)
        {
            if (ModelState.IsValid)
            {
                _context.Add(genManagers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Id", genManagers.CountryId);
            return View(genManagers);
        }

        // GET: GenManagers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genManagers = await _context.GenManagers.FindAsync(id);
            if (genManagers == null)
            {
                return NotFound();
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", genManagers.CountryId);
            return View(genManagers);
        }

        // POST: GenManagers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,YearBirth,Information,CountryId")] GenManagers genManagers)
        {
            if (id != genManagers.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(genManagers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GenManagersExists(genManagers.Id))
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
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Id", genManagers.CountryId);
            return View(genManagers);
        }

        // GET: GenManagers/Delete/5
        public async Task<IActionResult> Delete(int? id, bool saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genManagers = await _context.GenManagers
                .AsNoTracking()
                .Include(g => g.Country)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (genManagers == null)
            {
                return NotFound();
            }

            if (saveChangesError == true)
            {
                ViewData["ErrorMessage"] = "Помилка видалення";
            }
            return View(genManagers);
        }

        // POST: GenManagers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var genManagers = await _context.GenManagers.FindAsync(id);
            if (genManagers == null) return RedirectToAction(nameof(Index));

            try
            {
                DeleteGenManagersInCompanies(genManagers.Id);
                _context.GenManagers.Remove(genManagers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch(DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }            
        }

        private bool GenManagersExists(int id)
        {
            return _context.GenManagers.Any(e => e.Id == id);
        }
        private async void DeleteGenManagersInCompanies(int id)
        {
            var genManager = await _context.GenManagers.FindAsync(id);

            foreach (var i in _context.Companies)
            {
                if (i.GenManagerId == genManager.Id) i.GenManagerId = null;
            }
        }
    }
}
