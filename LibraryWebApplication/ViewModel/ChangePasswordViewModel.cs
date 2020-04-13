using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibraryWebApplication.ViewModel
{
    public class ChangePasswordViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Пароль, який встановленно зараз")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Новий пароль")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Compare("NewPassword", ErrorMessage = "Паролi не збiгаються")]
        [Display(Name = "Пiдтвердiть новий пароль")]
        [DataType(DataType.Password)]
        public string NewPasswordConfirm { get; set; }

    }
}
