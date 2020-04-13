using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibraryWebApplication.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Рiк народження")]
        [DateValidation(ErrorMessage = "Діапазон дати від 1950 року до 2020 року")]
        [DataType(DataType.Date)]
        public DateTime Year { get; set; }

        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Compare("Password", ErrorMessage = "Паролi не збiгаються")]
        [Display(Name = "Пiдтвердiть пароль")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }

    }
}
