using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryWebApplication
{
    public partial class Countries
    {
        public Countries()
        {
            Companies = new HashSet<Companies>();
            Filials = new HashSet<Filials>();
            GenManagers = new HashSet<GenManagers>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Поле повинно бути заповненим")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Довжина назви повинна бути від 3-х до 50-и символів")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$", ErrorMessage = "Ви можете ввести тільки літери латиниці та пробіл. Перша буква повинна бути прописною")]
        [Display(Name = "Назва")]

        public string Name { get; set; }

        public virtual ICollection<Companies> Companies { get; set; }
        public virtual ICollection<Filials> Filials { get; set; }
        public virtual ICollection<GenManagers> GenManagers { get; set; }
    }
}
