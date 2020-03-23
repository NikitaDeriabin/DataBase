using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryWebApplication
{
    public partial class Colors
    {
        public Colors()
        {
            ModelsOfProduct = new HashSet<ModelsOfProduct>();
        }

        public int Id { get; set; }

        [Display(Name = "Колір")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$", ErrorMessage = "Ви можете ввести тільки літери латиниці та пробіл. Перша буква повинна бути прописною")]

        public string Name { get; set; }

        public virtual ICollection<ModelsOfProduct> ModelsOfProduct { get; set; }
    }
}
