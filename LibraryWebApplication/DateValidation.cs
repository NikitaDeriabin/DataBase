using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibraryWebApplication
{
    public class DateValidation : ValidationAttribute
    {
        DateTime date;
        private readonly DateTime _downDateLine = new DateTime(1950, 1, 1, 23, 59, 59);
        private readonly DateTime _topDateLine = DateTime.Now;
        public override bool IsValid(object value)
        {
            if (value != null)
            {
                date = Convert.ToDateTime(value);
                if (date > _downDateLine && date < _topDateLine) return true;

            }
            return false;
        }
    }
}
