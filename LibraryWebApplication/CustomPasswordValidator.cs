using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;


namespace LibraryWebApplication.Models
{
    public class CustomPasswordValidator: IPasswordValidator<User>
    {
        public int RequiredLength { get; set; }//min length

        public CustomPasswordValidator(int length)
        {
            RequiredLength = length;
        }

        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string password)
        {
            List<IdentityError> errors = new List<IdentityError>();

            if(String.IsNullOrEmpty(password) || password.Length < RequiredLength)
            {
                errors.Add(new IdentityError
                {
                    Description = $"Мінімальна довжина пароля дорівнює {RequiredLength}"
                });
            }

            string pattern = @"(?=.*[0-9])(?=.*[!@#$%^&*_])(?=.*[a-z])(?=.*[A-Z])[0-9a-zA-Z!@#$%^&*_]*$";

            if(!Regex.IsMatch(password, pattern))
            {
                errors.Add(new IdentityError
                {
                    Description = "Пароль повинен містити літери латиниці. " +
                    "Літери верхнього та нижнього регістру. " +
                    "Цифри. " +
                    "Спеціальні символи. "
                });
            }

            return Task.FromResult(errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
        }
    }
}
