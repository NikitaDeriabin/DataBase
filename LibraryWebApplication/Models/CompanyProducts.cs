using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryWebApplication
{
    public partial class CompanyProducts
    {
        public CompanyProducts()
        {
            ModelsOfProduct = new HashSet<ModelsOfProduct>();
        }

        public int Id { get; set; }

        [Display(Name = "Компанія")]
        public int? CompanyId { get; set; }
        [Display(Name = "Категорія")]
        public int? ProductId { get; set; }

        public virtual Companies Company { get; set; }
        public virtual Products Product { get; set; }
        public virtual ICollection<ModelsOfProduct> ModelsOfProduct { get; set; }
    }
}
