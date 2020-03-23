using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryWebApplication
{
    public partial class ModelsOfProduct
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле повинно бути заповненим")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Довжина назви повинна бути від 3-х до 50-и символів")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z0-9""'\s-]*$", ErrorMessage = "Ви можете ввести тільки літери латиниці, цифри та пробіл. Перша буква повинна бути прописною")]
        [Display(Name = "Назва")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле повинно бути заповненим")]
        [Display(Name = "Ціна")]
        [Range(1, 10000000, ErrorMessage = "Значення має бути від 1 до 10000000"),  DataType(DataType.Currency, ErrorMessage = "Некоректний формат")]
        //[RegularExpression(@"[1-9,]+(\.[0-9]{2})?", ErrorMessage = "Значення не може починатися з нуля")]
        public double Price { get; set; }
        [Display(Name = "Інформація")]
        public string Information { get; set; }
        public int? CompProdId { get; set; }
        [Display(Name = "Колір")]
        public int? ColorId { get; set; }

        public virtual Colors Color { get; set; }
        public virtual CompanyProducts CompProd { get; set; }
    }
}
