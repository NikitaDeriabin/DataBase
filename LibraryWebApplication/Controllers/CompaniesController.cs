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

namespace LibraryWebApplication.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly DBLibraryContext _context;

        public CompaniesController(DBLibraryContext context)
        {
            _context = context;
        }

        // GET: Companies
        public async Task<IActionResult> Index(string searchString = "")
        {
            ViewData["CurrentFilter"] = searchString;


            var companies = from c in _context.Companies
                            select c;
            if(!String.IsNullOrEmpty(searchString))
            {
                companies = companies.Where(s => s.Name.Contains(searchString) || s.Country.Name.Contains(searchString)
                                            || s.GenManager.Name.Contains(searchString))
                                            .Include(c => c.Country).Include(c => c.GenManager);
                return View(await companies.ToListAsync());
            }

                
            var dBLibraryContext = _context.Companies.Include(c => c.Country).Include(c => c.GenManager);
            return View(await dBLibraryContext.ToListAsync());
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companies = await _context.Companies
                .Include(c => c.Country)
                .Include(c => c.GenManager)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (companies == null)
            {
                return NotFound();
            }

            //return View(companies);
            return RedirectToAction("Index", "Filials", new { id = companies.Id, name = companies.Name });
        }

        // GET: Companies/Categories/5
        public async Task<IActionResult> Categories(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companies = await _context.Companies
                .Include(c => c.Country)
                .Include(c => c.GenManager)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (companies == null)
            {
                return NotFound();
            }

            //return View(companies);
            return RedirectToAction("IndexForCompany", "CompanyProducts", new { id = companies.Id, name = companies.Name});
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name");
            ViewData["GenManagerId"] = new SelectList(_context.GenManagers, "Id", "Name");
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Year,CountryId,GenManagerId")] Companies companies)
        {
            if (ModelState.IsValid)
            {
                var company = _context.Companies.Where(c => c.GenManagerId == companies.GenManagerId && c.GenManagerId != null).FirstOrDefault();
                if (company != null)
                {
                    ModelState.AddModelError(string.Empty, "У компанії може бути лише один ген.диретор. Обраний ген.директор має компанію");
                }
                else
                {
                    _context.Add(companies);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", companies.CountryId);
            ViewData["GenManagerId"] = new SelectList(_context.GenManagers, "Id", "Name", companies.GenManagerId);
            return View(companies);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companies = await _context.Companies.FindAsync(id);
            if (companies == null)
            {
                return NotFound();
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", companies.CountryId);
            ViewData["GenManagerId"] = new SelectList(_context.GenManagers, "Id", "Name", companies.GenManagerId);
            return View(companies);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Year,CountryId,GenManagerId")] Companies companies)
        {
            if (id != companies.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(companies);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompaniesExists(companies.Id))
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

            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", companies.CountryId);
            ViewData["GenManagerId"] = new SelectList(_context.GenManagers, "Id", "Name", companies.GenManagerId);
            return View(companies);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id, bool saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companies = await _context.Companies
                .Include(c => c.Country)
                .Include(c => c.GenManager)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (companies == null)
            {
                return NotFound();
            }

            if(saveChangesError == true)
            {
                ViewData["ErrorMessage"] = "Помилка видалення";
            }

            return View(companies);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var companies = await _context.Companies.FindAsync(id);
            if (companies == null) return NotFound();

            try
            {
                DeleteFilialsAndCompanyProductsAndModelsOfProducts(id);
                _context.Companies.Remove(companies);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        private async void DeleteFilialsAndCompanyProductsAndModelsOfProducts(int id)
        {
            var company = await _context.Companies.FindAsync(id);

            //Filials
            foreach (var i in _context.Filials)
            {
                if (i.CompanyId == company.Id) _context.Filials.Remove(i);
            }

            //Categories
            foreach(var i in _context.CompanyProducts)
            {
                if(i.CompanyId == company.Id)
                {
                    foreach(var j in _context.ModelsOfProduct)
                    {
                        if (j.CompProdId == i.Id) _context.ModelsOfProduct.Remove(j);
                    }
                    _context.CompanyProducts.Remove(i);
                }
            }
        }

        private bool CompaniesExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }

    }
}
