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
    public class ModelsOfProductsController : Controller
    {
        private readonly DBLibraryContext _context;

        public ModelsOfProductsController(DBLibraryContext context)
        {
            _context = context;
        }

        // GET: ModelsOfProducts
        public async Task<IActionResult> Index(int? id, int? companyId, int? productId, string searchString = "")
        {
            //if (id == null) return RedirectToAction("Index", "Products");

            ViewBag.CompProdId = id;
            ViewBag.CompanyProductCompanyId = companyId;
            ViewBag.CompanyProductProductId = productId;
            ViewBag.CompanyProductCompanyName = _context.Companies.Where(c => c.Id == companyId).FirstOrDefault().Name;
            ViewBag.CompanyProductProductName = _context.Products.Where(c => c.Id == productId).FirstOrDefault().Name;

            ViewData["currentFilter"] = searchString;

            if(!String.IsNullOrEmpty(searchString))
            {
                var modelsOfProducts = _context.ModelsOfProduct.Where(b => b.CompProdId == id);

                modelsOfProducts = modelsOfProducts.Where(m => m.Name.Contains(searchString) || m.Color.Name.Contains(searchString)).Include(c => c.Color);
                return View(await modelsOfProducts.ToListAsync());
            }

            var modelsByCompanyProducts = _context.ModelsOfProduct.Where(b => b.CompProdId == id).Include(c => c.Color);

            //var dBLibraryContext = _context.ModelsOfProduct.Include(m => m.Color).Include(m => m.CompProd);
            return View(await modelsByCompanyProducts.ToListAsync());
        }

        // GET: ModelsOfProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelsOfProduct = await _context.ModelsOfProduct
                .Include(m => m.Color)
                .Include(m => m.CompProd)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelsOfProduct == null)
            {
                return NotFound();
            }

            var compProdId = modelsOfProduct.CompProdId;
            var CompProd = _context.CompanyProducts.Where(c => c.Id == compProdId).FirstOrDefault();
            ViewBag.CompProdId = compProdId;
            ViewBag.CompProdCompanyId = CompProd.CompanyId;
            ViewBag.CompProdProductId = CompProd.ProductId;

            return View(modelsOfProduct);
        }

        // GET: ModelsOfProducts/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create(int compProdId)
        {

            ViewData["ColorId"] = new SelectList(_context.Colors, "Id", "Name");
            //ViewData["CompProdId"] = new SelectList(_context.CompanyProducts, "Id", "Id");
            var CompProd = _context.CompanyProducts.Where(c => c.Id == compProdId).FirstOrDefault();

            ViewBag.CompProdId = compProdId;
            ViewBag.CompProdCompanyId = CompProd.CompanyId;
            ViewBag.CompProdProductId = CompProd.ProductId;

            return View();
        }

        // POST: ModelsOfProducts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int compProdId, [Bind("Id,Name,Price,Information,CompProdId,ColorId")] ModelsOfProduct modelsOfProduct)
        {
            modelsOfProduct.CompProdId = compProdId;
            var CompProd = _context.CompanyProducts.Where(c => c.Id == compProdId).FirstOrDefault();

            if (ModelState.IsValid)
            {
                _context.Add(modelsOfProduct);  
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Index", "ModelsOfProducts", new { id = modelsOfProduct.CompProdId, companyId = CompProd.CompanyId, productId = CompProd.ProductId });
            }
            //ViewData["ColorId"] = new SelectList(_context.Colors, "Id", "Id", modelsOfProduct.ColorId);
            //ViewData["CompProdId"] = new SelectList(_context.CompanyProducts, "Id", "Id", modelsOfProduct.CompProdId);
            //return View(modelsOfProduct);
            return RedirectToAction("Index", "ModelsOfProducts", new { id = modelsOfProduct.CompProdId, companyId = CompProd.CompanyId, productId = CompProd.ProductId });


        }

        // GET: ModelsOfProducts/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelsOfProduct = await _context.ModelsOfProduct.FindAsync(id);
           

            if (modelsOfProduct == null)
            {
                return NotFound();
            }
            var compProdId = modelsOfProduct.CompProdId;
            var CompProd = _context.CompanyProducts.Where(c => c.Id == compProdId).FirstOrDefault();
            ViewBag.CompProdId = compProdId;
            

            ViewData["ColorId"] = new SelectList(_context.Colors, "Id", "Name", modelsOfProduct.ColorId);
            ViewData["CompProdId"] = new SelectList(_context.CompanyProducts, "Id", "Id", modelsOfProduct.CompProdId);
            ViewBag.CompProdId = compProdId;
            ViewBag.CompProdCompanyId = CompProd.CompanyId;
            ViewBag.CompProdProductId = CompProd.ProductId;

            return View(modelsOfProduct);
        }

        // POST: ModelsOfProducts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Information,CompProdId,ColorId")] ModelsOfProduct modelsOfProduct)
        {
            if (id != modelsOfProduct.Id)
            {
                return NotFound();
            }
            var compProdId = modelsOfProduct.CompProdId;
            var CompProd = _context.CompanyProducts.Where(c => c.Id == compProdId).FirstOrDefault();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(modelsOfProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModelsOfProductExists(modelsOfProduct.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Index", "ModelsOfProducts", new { id = modelsOfProduct.CompProdId, companyId = CompProd.CompanyId, productId = CompProd.ProductId });

            }
            ViewData["ColorId"] = new SelectList(_context.Colors, "Id", "Id", modelsOfProduct.ColorId);
            ViewData["CompProdId"] = new SelectList(_context.CompanyProducts, "Id", "Id", modelsOfProduct.CompProdId);
            //return View(modelsOfProduct);
            return RedirectToAction("Index", "ModelsOfProducts", new { id = modelsOfProduct.CompProdId, companyId = CompProd.CompanyId, productId = CompProd.ProductId });

        }

        // GET: ModelsOfProducts/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelsOfProduct = await _context.ModelsOfProduct
                .Include(m => m.Color)
                .Include(m => m.CompProd)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelsOfProduct == null)
            {
                return NotFound();
            }
            var compProdId = modelsOfProduct.CompProdId;
            var CompProd = _context.CompanyProducts.Where(c => c.Id == compProdId).FirstOrDefault();
            ViewBag.CompProdId = compProdId;
            ViewBag.CompProdCompanyId = CompProd.CompanyId;
            ViewBag.CompProdProductId = CompProd.ProductId;

            return View(modelsOfProduct);
        }

        // POST: ModelsOfProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var modelsOfProduct = await _context.ModelsOfProduct.FindAsync(id);
            _context.ModelsOfProduct.Remove(modelsOfProduct);
            await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
            var compProdId = modelsOfProduct.CompProdId;
            var CompProd = _context.CompanyProducts.Where(c => c.Id == compProdId).FirstOrDefault();
            return RedirectToAction("Index", "ModelsOfProducts", new { id = modelsOfProduct.CompProdId, companyId = CompProd.CompanyId, productId = CompProd.ProductId });
        }

        private bool ModelsOfProductExists(int id)
        {
            return _context.ModelsOfProduct.Any(e => e.Id == id);
        }
    }
}
