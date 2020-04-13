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
    public class CompanyProductsController : Controller
    {
        private readonly DBLibraryContext _context;

        public CompanyProductsController(DBLibraryContext context)
        {
            _context = context;
        }

        // GET: CompanyProducts
        public async Task<IActionResult> Index(int? id, string? name)
        {
            if (id == null) return RedirectToAction("Index", "Products");
            ViewBag.ProductId = id;
            ViewBag.ProductName = name;
            var companyProductByProduct = _context.CompanyProducts.Where(b => b.ProductId == id).Include(b => b.Product).Include(b => b.Company);

            return View(await companyProductByProduct.ToListAsync());
            //var dBLibraryContext = _context.CompanyProducts.Include(c => c.Company).Include(c => c.Product);
            //return View(await dBLibraryContext.ToListAsync());
        }

        public async Task<IActionResult> IndexForCompany(int? id, string? name)
        {
            if (id == null) return RedirectToAction("Index", "Companies");
            ViewBag.CompanyId = id;
            ViewBag.CompanyName = name;
            var companyProductByProduct = _context.CompanyProducts.Where(b => b.CompanyId == id).Include(b => b.Product).Include(b => b.Company);

            return View(await companyProductByProduct.ToListAsync());
            //var dBLibraryContext = _context.CompanyProducts.Include(c => c.Company).Include(c => c.Product);
            //return View(await dBLibraryContext.ToListAsync());
        }

        // GET: CompanyProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyProducts = await _context.CompanyProducts
                .Include(c => c.Company)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            //string? company_name = _context.Companies.Where(c => c.Id == companyProducts.Id).FirstOrDefault().Name;
            //string? product_name = _context.Products.Where(c => c.Id == companyProducts.Id).FirstOrDefault().Name;
            if (companyProducts == null)
            {
                return NotFound();
            }

            //return View(companyProducts);
            return RedirectToAction("Index", "ModelsOfProducts", new { id = companyProducts.Id, companyId = companyProducts.CompanyId, productId = companyProducts.ProductId });

        }

        // GET: CompanyProducts/Create
        [Authorize(Roles = "admin")]
        public IActionResult CreateForCompany(int companyId)
        {
            //ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            ViewBag.CompanyId = companyId;
            ViewBag.CompanyName = _context.Companies.Where(c => c.Id == companyId).FirstOrDefault().Name;

            return View();
        }

        // POST: CompanyProducts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateForCompany(int companyId, [Bind("Id,CompanyId,ProductId")] CompanyProducts companyProducts)
        {
            companyProducts.CompanyId = companyId;
            if (ModelState.IsValid)
            {
                var comp_prod = _context.CompanyProducts.Where(c => c.ProductId == companyProducts.ProductId).Where(c => c.CompanyId == companyId).FirstOrDefault();
                if (comp_prod != null)
                {
                    ModelState.AddModelError("Error", "Для цієї компанії вже існує дана категорія");
                }
                else
                {
                    _context.Add(companyProducts);
                    await _context.SaveChangesAsync();
                    //return RedirectToAction(nameof(Index));
                    return RedirectToAction("IndexForCompany", "CompanyProducts", new { id = companyId, name = _context.Companies.Where(c => c.Id == companyId).FirstOrDefault().Name });
                }
            }
            //ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", companyProducts.CompanyId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", companyProducts.ProductId);
            //return View(companyProducts);
            return RedirectToAction("IndexForCompany", "CompanyProducts", new { id = companyId, name = _context.Companies.Where(c => c.Id == companyId).FirstOrDefault().Name });
        }

        // GET: CompanyProducts/Create
        [Authorize(Roles = "admin")]

        public IActionResult Create(int productId)
        {
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name");
            //ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            ViewBag.ProductId = productId;
            ViewBag.ProductName = _context.Products.Where(c => c.Id == productId).FirstOrDefault().Name;
            return View();
        }

        // POST: CompanyProducts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int productId, [Bind("Id,CompanyId,ProductId")] CompanyProducts companyProducts)
        {
            companyProducts.ProductId = productId;
            if (ModelState.IsValid)
            {
                var comp_prod = _context.CompanyProducts.Where(c => c.ProductId == productId).Where(c => c.CompanyId == companyProducts.CompanyId).FirstOrDefault();
                if (comp_prod != null)
                {
                    ModelState.AddModelError("Error", "В цій категорії присутня обрана компанія");
                }
                else
                {
                    _context.Add(companyProducts);
                    await _context.SaveChangesAsync();
                    //return RedirectToAction(nameof(Index));
                    return RedirectToAction("Index", "CompanyProducts", new { id = productId, name = _context.Products.Where(c => c.Id == productId).FirstOrDefault().Name });
                }

            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", companyProducts.CompanyId);
            //ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", companyProducts.ProductId);
            //return View(companyProducts);
            return RedirectToAction("Index", "CompanyProducts", new { id = productId, name = _context.Products.Where(c => c.Id == productId).FirstOrDefault().Name });

        }

        // GET: CompanyProducts/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyProducts = await _context.CompanyProducts.FindAsync(id);
            ViewBag.Product_id = companyProducts.ProductId;
            ViewBag.ProductName = _context.Products.Where(c => c.Id == companyProducts.ProductId).FirstOrDefault().Name;
            if (companyProducts == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", companyProducts.CompanyId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", companyProducts.ProductId);
            return View(companyProducts);
        }

        // POST: CompanyProducts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CompanyId,ProductId")] CompanyProducts companyProducts)
        {
            if (id != companyProducts.Id)
            {
                return NotFound();
            }


            string? productName = _context.Products.Where(c => c.Id == companyProducts.ProductId).FirstOrDefault().Name;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(companyProducts);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyProductsExists(companyProducts.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Index", "CompanyProducts", new { id = companyProducts.ProductId, name = productName });

            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", companyProducts.CompanyId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", companyProducts.ProductId);
            return RedirectToAction("Index", "CompanyProducts", new { id = companyProducts.ProductId, name = productName });
            //return View(companyProducts);


        }

        // GET: CompanyProducts/DeleteForCompany/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteForCompany(int? id, bool saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyProducts = await _context.CompanyProducts
                .Include(c => c.Company)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            ViewBag.Company_id = companyProducts.CompanyId;
            ViewBag.CompanyName = _context.Companies.Where(c => c.Id == companyProducts.CompanyId).FirstOrDefault().Name;
            if (companyProducts == null)
            {
                return NotFound();
            }

            if (saveChangesError == true)
            {
                ViewData["ErrorMessage"] = "Помилка видалення";
            }

            return View(companyProducts);
        }

        // POST: CompanyProducts/DeleteForCompany/5
        [HttpPost, ActionName("DeleteForCompany")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedForCompany(int id)
        {
            var companyProducts = await _context.CompanyProducts.FindAsync(id);
            if (companyProducts == null) return NotFound();

            try
            {
                DeleteModelsOfProducts(id);
                _context.CompanyProducts.Remove(companyProducts);
                string? companyName = _context.Companies.Where(c => c.Id == companyProducts.CompanyId).FirstOrDefault().Name;
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("IndexForCompany", "CompanyProducts", new { id = companyProducts.CompanyId, name = companyName });
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(DeleteForCompany), new { id = id, saveChangesError = true });
            }

        }

        // GET: CompanyProducts/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id, bool saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyProducts = await _context.CompanyProducts
                .Include(c => c.Company)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            ViewBag.Product_id = companyProducts.ProductId;
            ViewBag.ProductName = _context.Products.Where(c => c.Id == companyProducts.ProductId).FirstOrDefault().Name;
            if (companyProducts == null)
            {
                return NotFound();
            }

            if (saveChangesError == true)
            {
                ViewData["ErrorMessage"] = "Помилка видалення";
            }

            return View(companyProducts);
        }

        // POST: CompanyProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var companyProducts = await _context.CompanyProducts.FindAsync(id);
            if (companyProducts == null) return NotFound();

            try
            {
                DeleteModelsOfProducts(id);
                _context.CompanyProducts.Remove(companyProducts);
                string? productName = _context.Products.Where(c => c.Id == companyProducts.ProductId).FirstOrDefault().Name;
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Index", "CompanyProducts", new { id = companyProducts.ProductId, name = productName });
            }
            catch(DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
           
        }

        private bool CompanyProductsExists(int id)
        {
            return _context.CompanyProducts.Any(e => e.Id == id);
        }

        private async void DeleteModelsOfProducts(int id)
        {
            var companyProduct = await _context.CompanyProducts.FindAsync(id);

            foreach (var i in _context.ModelsOfProduct)
            {
                if (i.CompProdId == id) _context.ModelsOfProduct.Remove(i);
            }
        }
    }
}
