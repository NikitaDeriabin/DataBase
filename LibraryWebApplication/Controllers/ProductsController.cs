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
using System.Drawing;
using System.Text.RegularExpressions;



namespace LibraryWebApplication.Controllers
{
    public class ProductsController : Controller
    {
        private readonly DBLibraryContext _context;
        private readonly string _regexCompany = @"^[A-Z]+[a-zA-Z""'\s-]*$";
        private readonly string _regexCategory = @"^[A-Z]+[a-zA-Z""'\s-]*$";
        private readonly string _regexModel = @"^[A-Z]+[a-zA-Z0-9""'\s-]*$";
        private readonly string _regexColor = @"^[A-Z]+[a-zA-Z""'\s-]*$";


        public ProductsController(DBLibraryContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(string searchString = "", string errorString = "")
        {
            if (errorString != "")
            {
                ViewData["ErrorMessage"] = errorString;
            }
            else
            {
                ViewData["CurrentFilter"] = searchString;
                var categories = from c in _context.Products
                                 select c;
                if (!String.IsNullOrEmpty(searchString))
                {
                    categories = categories.Where(s => s.Name.Contains(searchString));
                    return View(await categories.ToListAsync());
                }
            }

            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (products == null)
            {
                return NotFound();
            }

            //return View(products);
            return RedirectToAction("Index", "CompanyProducts", new { id = products.Id, name = products.Name });
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Information")] Products products)
        {
            if (ModelState.IsValid)
            {
                var product = _context.Products.Where(c => c.Name == products.Name).FirstOrDefault();
                if(product != null)
                {
                    ModelState.AddModelError(string.Empty, "Категорія з такою назвою вже існує");
                }
                else
                {
                    _context.Add(products);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(products);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }
            return View(products);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Information")] Products products)
        {
            if (id != products.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(products);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsExists(products.Id))
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
            return View(products);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id, bool saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (products == null)
            {
                return NotFound();
            }

            if(saveChangesError == true)
            {
                ViewData["ErrorMessage"] = "Помилка видалення";
            }

            return View(products);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var products = await _context.Products.FindAsync(id);
            if (products == null) return NotFound();

            try
            {
                DeleteCompanyProductsAndModelsOfProducts(id);
                _context.Products.Remove(products);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }


        }

        private async void DeleteCompanyProductsAndModelsOfProducts(int id)
        {
            var product = await _context.Products.FindAsync(id);

            foreach (var i in _context.CompanyProducts)
            {
                if (i.ProductId == product.Id)
                {
                    foreach (var j in _context.ModelsOfProduct)
                    {
                        if (j.CompProdId == i.Id) _context.ModelsOfProduct.Remove(j);
                    }
                    _context.CompanyProducts.Remove(i);
                }
            }
        }

        private bool ProductsExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (fileExcel == null) throw new FileLoadException("Файл не обрано");
                    if (fileExcel != null)
                    {
                        using (var stream = new FileStream(fileExcel.FileName, FileMode.Create))
                        {
                            await fileExcel.CopyToAsync(stream);
                            using (XLWorkbook workBook = new XLWorkbook(stream, XLEventTracking.Disabled))
                            {
                                foreach (IXLWorksheet worksheet in workBook.Worksheets)
                                {
                                    Products newCategory;
                                    var c = (from cat in _context.Products
                                             where cat.Name.Contains(worksheet.Name)
                                             select cat).ToList();
                                    if (c.Count > 0)
                                    {
                                        newCategory = c[0];
                                    }
                                    else
                                    {
                                        if (Regex.IsMatch(worksheet.Name, _regexCategory) == false) throw new Exception("Некоректна назва аркуша");
                                        newCategory = new Products();
                                        newCategory.Name = worksheet.Name;
                                        _context.Products.Add(newCategory);
                                    }
                                    //await _context.SaveChangesAsync();
                                    
                                    CheckNameRow(worksheet);//проверка рядка с названиями столбцов
                                                            //просмотр всех рядков
                                    foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                                    {

                                        if (Regex.IsMatch(row.Cell(1).Value.ToString(), _regexModel) == false) throw new Exception("Некоректна назва товару");
                                        ModelsOfProduct Model = new ModelsOfProduct();
                                        Model.Name = row.Cell(1).Value.ToString();
                                        if (Model.Name == "") throw new Exception("Порожнє поле назви товару");
                                        if (row.Cell(3).Value.ToString() != "") Model.Price = Convert.ToDouble(row.Cell(3).Value);
                                        else throw new Exception("Порожнє поле ціни");
                                        Model.Information = row.Cell(4).Value.ToString();

                                        if (row.Cell(2).Value.ToString().Length > 0)
                                        {
                                            Companies Company;
                                            var comp = (from cmp in _context.Companies
                                                        where cmp.Name.Contains(row.Cell(2).Value.ToString())
                                                        select cmp).ToList();
                                            if (comp.Count > 0)
                                            {
                                                Company = comp[0];
                                            }
                                            else
                                            {
                                                if (Regex.IsMatch(row.Cell(2).Value.ToString(), _regexCompany) == false) throw new Exception("Некоректна назва компанії");
                                                Company = new Companies();
                                                Company.Name = row.Cell(2).Value.ToString();
                                                _context.Companies.Add(Company);
                                            }
                                            //await _context.SaveChangesAsync();

                                            if (row.Cell(5).Value.ToString().Length > 0)
                                            {
                                                Colors Color;
                                                var clr = (from col in _context.Colors
                                                            where col.Name.Contains(row.Cell(5).Value.ToString())
                                                            select col).ToList();
                                                if (clr.Count > 0)
                                                {
                                                    Color = clr[0];
                                                }
                                                else
                                                {
                                                    if (Regex.IsMatch(row.Cell(5).Value.ToString(), _regexColor) == false) throw new Exception("Некоректна назва кольору");
                                                    Color = new Colors();
                                                    Color.Name = row.Cell(5).Value.ToString();
                                                    _context.Colors.Add(Color);
                                                }
                                                Model.Color = Color;

                                                CompanyProducts companyProducts;
                                                if (!IsExistCompanyInProduct(newCategory, Company))
                                                {
                                                    companyProducts = new CompanyProducts();
                                                    companyProducts.Product = newCategory;
                                                    companyProducts.Company = Company;
                                                    _context.CompanyProducts.Add(companyProducts);
                                                }
                                                else
                                                {
                                                    companyProducts = _context.CompanyProducts.Where(c => c.CompanyId == Company.Id && c.ProductId == newCategory.Id).FirstOrDefault();
                                                }
                                                await _context.SaveChangesAsync();
                                                Model.CompProdId = companyProducts.Id;
                                            }
                                            else throw new Exception("Порожнє поле кольору");
                                        }
                                        else throw new Exception("Порожня назва компанії");
                                        if (!IsExistModel(Model)) _context.ModelsOfProduct.Add(Model);
                                    }
                                    
                                    
                                }
                            }


                        }

                    }
                }
                catch (ArgumentException ex)
                {
                    //неправильное название столбцов
                    return RedirectToAction("Index", "Products", new { searchString = "",  errorString = ex.Message });
                }
                catch (FileLoadException ex)
                {
                    //ошибка загрузки файла или файл не выбран
                    return RedirectToAction("Index", "Products", new { errorString = ex.Message });

                }
                catch (Exception ex)
                {
                    //некоретные данные
                    if(ex.Message == "") return RedirectToAction("Index", "Products", new { errorString = "Помилка завантаження файлу" });
                    return RedirectToAction("Index", "Products", new { errorString = ex.Message });

                }


                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Export(string searchString ="")
        {
            using (XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled))
            {   
                var filter = searchString;

                var categories = _context.Products.Where(c => c.Name.Contains(filter.ToString())).ToList();

                foreach (var c in categories)
                {
                    var worksheet = workbook.Worksheets.Add(c.Name);

                    worksheet.Cell("A1").Value = "Name";
                    worksheet.Cell("B1").Value = "Company";
                    worksheet.Cell("C1").Value = "Price";
                    worksheet.Cell("D1").Value = "Info";
                    worksheet.Cell("E1").Value = "Color";
                    worksheet.Row(1).Style.Font.Bold = true;
                    worksheet.Range("A1","E1").Style.Fill.BackgroundColor = XLColor.LightGreen;

                    var compProd = GetCompProdInProducts(c);

                    int k = 2;
                    foreach(var i in compProd)
                    {
                        var models = _context.ModelsOfProduct.Where(m => m.CompProdId == i.Id).ToList();
                        foreach(var j in models)
                        {
                            worksheet.Cell(k, 1).Value = j.Name;
                            worksheet.Cell(k, 2).Value = _context.Companies.Where(c => c.Id == i.CompanyId).Select(c => c.Name);
                            worksheet.Cell(k, 3).Value = j.Price;
                            worksheet.Cell(k, 4).Value = j.Information;
                            worksheet.Cell(k, 5).Value = _context.Colors.Where(c => c.Id == j.ColorId).Select(c => c.Name);
                            k++;
                        }
                    }
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();

                    return new FileContentResult(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"library_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };
                }

            }

        }

        private bool IsExistCompanyInProduct(Products category, Companies company)
        {
            foreach (var i in _context.CompanyProducts)
            {
                if (i.ProductId == category.Id && i.CompanyId == company.Id) return true;
            }
            return false;
        }

        private bool IsExistModel(ModelsOfProduct model)
        {
            foreach (var i in _context.ModelsOfProduct)
            {
                if (String.Compare(i.Name.ToUpper(), model.Name.ToUpper()) == 0 && model.Price == i.Price && model.Color == i.Color) return true;
            }
            return false;
        }

        private void CheckNameRow(IXLWorksheet worksheet) 
        {
            if (worksheet.Row(1).Cell(1).Value.ToString() != "Name") throw new ArgumentException("Некоректна назва стовпців");
            if (worksheet.Row(1).Cell(2).Value.ToString() != "Company") throw new ArgumentException("Некоректна назва стовпців");
            if (worksheet.Row(1).Cell(3).Value.ToString() != "Price") throw new ArgumentException("Некоректна назва стовпців");
            if (worksheet.Row(1).Cell(4).Value.ToString() != "Info") throw new ArgumentException("Некоректна назва стовпців");
            if (worksheet.Row(1).Cell(5).Value.ToString() != "Color") throw new ArgumentException("Некоректна назва стовпців");
        }

        private List<CompanyProducts> GetCompProdInProducts(Products cat)
        {
            var comp = _context.CompanyProducts.Where(c => c.ProductId == cat.Id).ToList();
            return comp;
        }
    }
}
