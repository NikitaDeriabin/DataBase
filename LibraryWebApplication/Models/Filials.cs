using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace LibraryWebApplication
{
    public partial class Filials
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Довжина назви повинна бути від 3-х до 50-и символів")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z_""'\s-]*$", ErrorMessage = "Ви можете ввести тільки літери латиниці, нижнє підкреслювання та пробіл. Перша буква повинна бути прописною")]
        [Display(Name = "Філіал")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле повинно бути заповненим")]
        [Range(1, 20, ErrorMessage = "Кількість має бути від 1 до 20")]
        [Display(Name = "Кількість")]
        public int? Amount { get; set; }

        [Display(Name = "Компанія")]
        public int? CompanyId { get; set; }

        [Display(Name = "Країна")]
        public int? CountryId { get; set; }

        public virtual Companies Company { get; set; }
        public virtual Countries Country { get; set; }
    }
}
