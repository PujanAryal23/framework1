using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nucleus.Service.Support.Utils
{
    public static class InputValidation
    {
        public static bool CheckDateFormat(string date)
        {
            return UIAutomation.Framework.Utils.InputValidation.IsDateInFormat(date);
        }
    }

}
