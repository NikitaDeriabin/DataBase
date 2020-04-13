using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryWebApplication.Models;
using Microsoft.AspNetCore.Identity;

namespace LibraryWebApplication
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            string moderatorEmail = "moderator@gmail.com";
            string password = "Qwerty_1";

            if(await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }
            if(await roleManager.FindByNameAsync("user") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("user"));
            }
            if (await roleManager.FindByNameAsync("moderator") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("moderator"));
            }
            if (await userManager.FindByNameAsync(moderatorEmail) == null)
            {
                User moderator = new User { Email = moderatorEmail, UserName = moderatorEmail, Year = new DateTime(2001, 12, 4) };
                IdentityResult result = await userManager.CreateAsync(moderator, password);
                if(result.Succeeded)
                {
                    await userManager.AddToRoleAsync(moderator, "moderator");
                }
            }
        }
    }
}
