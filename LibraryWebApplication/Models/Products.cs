using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryWebApplication
{
    public partial class Products
    {
        public Products()
        {
            CompanyProducts = new HashSet<CompanyProducts>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Поле повинно бути заповненим")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Довжина назви повинна бути від 2-х до 50-и символів")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$", ErrorMessage = "Ви можете ввести тільки літери латиниці та пробіл. Перша буква повинна бути прописною")]
        [Display(Name = "Назва")]
        public string Name { get; set; }
        [Display(Name = "Інформація")]
        public string Information { get; set; }

        public virtual ICollection<CompanyProducts> CompanyProducts { get; set; }
    }
}
