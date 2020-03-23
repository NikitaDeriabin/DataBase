using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryWebApplication
{
    public partial class GenManagers
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле повинно бути заповненим")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Довжина імені повинна бути від 3-х до 50-и символів")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$", ErrorMessage = "Ви можете ввести тільки літери латиниці та пробіл. Перша буква повинна бути прописною")]
        [Display(Name = "ПІБ")]
        public string Name { get; set; }

        
        [DateValidation(ErrorMessage = "Діапазон дати від 1950 року до 2020 року")]
        [DataType(DataType.Date)]
        [Display(Name = "Дата народження")]
        public DateTime? YearBirth { get; set; }

        [Display(Name = "Додаткова інформація")]
        public string Information { get; set; }

        [Display(Name = "Країна")]
        public int? CountryId { get; set; }

        public virtual Countries Country { get; set; }
        public virtual Companies Companies { get; set; }
    }
}
