using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        private readonly DBLibraryContext _context;

        public ChartsController(DBLibraryContext context)
        {
            _context = context;
        }

        [HttpGet("JsonData")]

        public JsonResult JsonData()
        {
            var companies = _context.Companies.Include(f => f.Filials).ToList();
            List<object> compFilials = new List<object>();

            compFilials.Add(new[] { "Компанії", "Кількість філіалів" });

            foreach (var c in companies)
            {
                int? amount = 0;
                foreach(var i in c.Filials)
                {
                    amount += i.Amount;
                }
                compFilials.Add(new object[] { c.Name, amount });
            }

            return new JsonResult(compFilials);
        }

        [HttpGet("JsonDataForCategories")]

        public JsonResult JsonDataForCategories()
        {
            var categories = _context.Products.Include(f => f.CompanyProducts).ToList();
            List<object> prodCategories = new List<object>();

            prodCategories.Add(new[] { "Категорії", "Кількість товарів" });

           foreach (var i in categories)
            {               
                int amount = 0;
                foreach (var j in i.CompanyProducts)
                {
                    amount += _context.ModelsOfProduct.Where(c => c.CompProdId == j.Id).Count();
                }
                prodCategories.Add(new object[] { i.Name, amount});
            }
            return new JsonResult(prodCategories);
        }

    }

}