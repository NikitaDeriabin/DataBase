using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace LibraryWebApplication.ViewModel
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Рiк народження")]
        [DateValidation(ErrorMessage = "Діапазон дати від 1950 року до 2020 року")]
        [DataType(DataType.Date)]
        public DateTime Year { get; set; }
    }
}
