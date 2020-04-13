using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryWebApplication;
using Microsoft.AspNetCore.Http;
using System.IO;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;

namespace LibraryWebApplication.Controllers
{
    public class CountriesController : Controller
    {
        private readonly DBLibraryContext _context;

        public CountriesController(DBLibraryContext context)
        {
            _context = context;
        }

        // GET: Countries
        public async Task<IActionResult> Index()
        {
            return View(await _context.Countries.ToListAsync());
        }

        // GET: Countries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var countries = await _context.Countries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (countries == null)
            {
                return NotFound();
            }

            return View(countries);
        }

        // GET: Countries/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Countries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Countries countries)
        {
            if (ModelState.IsValid)
            {
                var country = _context.Countries.Where(c => c.Name == countries.Name).FirstOrDefault();
                if (country != null)
                {
                    ModelState.AddModelError(string.Empty, "Країна з такою назвою вже існує");
                }
                else
                {
                    _context.Add(countries);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(countries);
        }

        // GET: Countries/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var countries = await _context.Countries.FindAsync(id);
            if (countries == null)
            {
                return NotFound();
            }
            return View(countries);
        }

        // POST: Countries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Countries countries)
        {
            if (id != countries.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(countries);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountriesExists(countries.Id))
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
            return View(countries);
        }

        // GET: Countries/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id, bool saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var countries = await _context.Countries
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (countries == null)
            {
                return NotFound();
            }
            if (saveChangesError == true)
            {
                ViewData["ErrorMessage"] = "Помилка видалення";
            }
            return View(countries);
        }

        // POST: Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var countries = await _context.Countries.FindAsync(id);
            if (countries == null) return RedirectToAction(nameof(Index));

            try
            {
                DeleteCountriesInGenManagersAndCompanies(countries.Id);
                _context.Countries.Remove(countries);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                //return RedirectToAction(nameof(Index));
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }

        }

        private bool CountriesExists(int id)
        {
            return _context.Countries.Any(e => e.Id == id);
        }

        private async void DeleteCountriesInGenManagersAndCompanies(int id)
        {
            var country = await _context.Countries.FindAsync(id);

            foreach (var i in _context.Companies)
            {
                if (i.CountryId == country.Id) i.CountryId = null;
            }

            foreach (var i in _context.GenManagers)
            {
                if (i.CountryId == country.Id) i.CountryId = null;
            }

            foreach (var i in _context.Filials)
            {
                if (i.CountryId == country.Id) i.CountryId = null;
            }

        }

    }
}
