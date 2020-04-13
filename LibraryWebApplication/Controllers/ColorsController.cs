using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryWebApplication;
using Microsoft.AspNetCore.Authorization;

namespace LibraryWebApplication.Controllers
{
    public class ColorsController : Controller
    {
        private readonly DBLibraryContext _context;

        public ColorsController(DBLibraryContext context)
        {
            _context = context;
        }

        // GET: Colors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Colors.ToListAsync());
        }

        // GET: Colors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var colors = await _context.Colors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (colors == null)
            {
                return NotFound();
            }

            return View(colors);
        }

        // GET: Colors/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Colors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Colors colors)
        {
            if (ModelState.IsValid)
            {
                var color = _context.Colors.Where(c => c.Name == colors.Name).FirstOrDefault();
                if (color != null)
                {
                    ModelState.AddModelError(string.Empty, "Колір з такою назвою вже існує");
                }
                else
                {
                    _context.Add(colors);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(colors);
        }

        // GET: Colors/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var colors = await _context.Colors.FindAsync(id);
            if (colors == null)
            {
                return NotFound();
            }
            return View(colors);
        }

        // POST: Colors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Colors colors)
        {
            if (id != colors.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(colors);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ColorsExists(colors.Id))
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
            return View(colors);
        }

        // GET: Colors/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id, bool saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var colors = await _context.Colors
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (colors == null)
            {
                return NotFound();
            }

            if (saveChangesError == true)
            {
                ViewData["ErrorMessage"] = "Помилка видалення";
            }

            return View(colors);
        }

        // POST: Colors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var colors = await _context.Colors.FindAsync(id);
            if (colors == null) return RedirectToAction(nameof(Index));

            try
            {
                DeleteColorsInModelsOfProducts(id);
                _context.Colors.Remove(colors);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }

        }

        private bool ColorsExists(int id)
        {
            return _context.Colors.Any(e => e.Id == id);
        }

        private async void DeleteColorsInModelsOfProducts(int id)
        {
            var color = await _context.Colors.FindAsync(id);
            
            foreach(var i in _context.ModelsOfProduct)
            {
                if (i.ColorId == color.Id) i.ColorId = null;
            }
        }
    }
}
