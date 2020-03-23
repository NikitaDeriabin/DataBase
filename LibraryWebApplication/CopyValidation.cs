using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibraryWebApplication
{
    public class CopyValidation : ValidationAttribute
    {
        private string str = null;
        public override bool IsValid(object value)
        {
            if(value != null)
            {
                str = value.ToString();
                var country = 2;
            }

            return false;
        }
    }
}
