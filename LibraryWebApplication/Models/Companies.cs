using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryWebApplication
{
    public partial class Companies
    {
        public Companies()
        {
            CompanyProducts = new HashSet<CompanyProducts>();
            Filials = new HashSet<Filials>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Довжина назви повинна бути від 3-х до 50-и символів")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$", ErrorMessage = "Ви можете ввести тільки літери латиниці та пробіл. Перша буква повинна бути прописною")]
        [Display(Name = "Компанія")]
        public string Name { get; set; }

        [DateValidation(ErrorMessage = "Діапазон дати від 1950 року до 2020 року")]
        [DataType(DataType.Date)]
        [Display(Name = "Рік заснування")]
        public DateTime? Year { get; set; }

        [Display(Name = "Країна")]
        public int? CountryId { get; set; }

        [Display(Name = "Ген.Директор")]
        public int? GenManagerId { get; set; }

        public virtual Countries Country { get; set; }
        public virtual GenManagers GenManager { get; set; }
        public virtual ICollection<CompanyProducts> CompanyProducts { get; set; }
        public virtual ICollection<Filials> Filials { get; set; }
    }
}
