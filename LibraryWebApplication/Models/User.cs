using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LibraryWebApplication.Models
{
    public class User : IdentityUser
    {
        [DataType(DataType.Date)]
        public DateTime Year { get; set; }
    }
}
