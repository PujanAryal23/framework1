using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace UIAutomation.Framework.Utils
{
    public static class InputValidation
    {

        /// <summary>
        /// Returns  if the date format is in mm/dd/yyyy format or not
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsDateInFormat(this string date)
        {
            //do something
            Console.WriteLine("Checking format of  Date: {0}" ,date);
            return Regex.IsMatch(date, @"^(0[1-9]|1[0-2])\/(0[1-9]|1\d|2\d|3[01])\/(19|20)\d{2}$");

        }
        /// <summary>
        /// Returns  if the date period  format is in mm/dd/yyyy - mm/dd/yyyy format or not
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsDatePeriodInFormat(this string date)
        {
            //do something
            Console.WriteLine("Checking format of Date Period: " + date);
            return Regex.IsMatch(date, @"^(0[1-9]|1[0-2])\/(0[1-9]|1\d|2\d|3[01])\/(19|20)\d{2}[ ][-][ ](0[1-9]|1[0-2])\/(0[1-9]|1\d|2\d|3[01])\/(19|20)\d{2}$");

        }

        /// <summary>
        /// Returns  if the date period  format is in mm/dd/yyyy h/m/t AM/PM  format or not
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsDateTimeInFormat(this string date)
        {
            //do something
            Console.WriteLine("Checking format of Date Time: " + date);
            var t = Regex.IsMatch(date, @"(0[1-9]|1[0-2])\/(0[1-9]|1\d|2\d|3[0-1])\/(19|20)\d{2} (0*[0-9]|1[0-2]):(0*[0-9]|1\d|2\d|3\d|4\d|5[0-9]):(0*[0-9]|1\d|2\d|3\d|4\d|5[0-9]) (AM|PM)$");
            return t;
        }

        /// <summary>
        /// Returns  if the date period  format is in mm/dd/yyyy h/m/t AM/PM  format or not
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsDateTimeWithoutSecInFormat(this string date)
        {
            Console.WriteLine("Checking format of Date Time: " + date);
            var t = Regex.IsMatch(date, @"(0[1-9]|1[0-2])\/(0[1-9]|1\d|2\d|3[0-1])\/(19|20)\d{2} (0*[0-9]|1[0-2]):(0*[0-9]|1\d|2\d|3\d|4\d|5[0-9]) (AM|PM)$");
            return t;
        }

        /// <summary>
        /// Returns  if the date period  format is in YYYY-MM-DDTHH:MM:SS  format or not
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsDateTimeInUTCFormat(this string date)
        {
            //do something
            Console.WriteLine("Checking format of Date Time: " + date);
            var t = Regex.IsMatch(date, @"^\d{4}-[01]{1}\d{1}-[0-3]{1}\d{1}T[0-2]{1}\d{1}:[0-6]{1}\d{1}:[0-6]{1}\d{1}$");
            return t;
        }
        //<First Name> <Last Name> (<username>)
        public static bool DoesNameContainsOnlyFirstWithLastname(this string name)
        {
            Console.WriteLine("Checking format of input name: " + name);
            return Regex.IsMatch(name,@"^(\S+ )+\S+$");
        }

        //<First Name> 
        public static bool DoesNameContainsOnlyFirsName(this string name)
        {
            Console.WriteLine("Checking format of input name: " + name);
            return Regex.IsMatch(name, @"^(\S+)$");
        }

        //<First Name> <Last Name> (<username>)
        public static bool DoesNameContainsNameWithUserName(this string name)
        {
            Console.WriteLine("Checking format of input name with username in parenthesis <FirstName LastName (UserId)>: " + name);
            return Regex.IsMatch(name, @"^(\S+ )+\S+ +\(+\S+\)+$");
        }

        public static bool IsDollarAmountInCorrectFormat(this string value)
        {
            Console.WriteLine("Checking format of Amount: " + value);
            return Regex.IsMatch(value, @"^\$[0-9.,]+ \([0-9]+%\)$");
        }

        ///<summary>
        /// (2 digit specialty code) - (specialty short description)
        ///</summary>
        /// <param name="value">value to validate</param>
        /// <returns>boolean for whether format is correct or not</returns>
        public static bool IsProviderSpecialtyInCorrectFormat(this string value)
        {
            Console.WriteLine("Checking format of ProviderSpecialty: " + value);
            return Regex.IsMatch(value, @"^(\d)+\s+\-+\s+\S+$");
        }
        ///<summary>
        /// (4 character) - (description)
        ///</summary>
        /// <param name="value">value to validate</param>
        /// <returns>boolean for whether format is correct or not</returns>
        public static bool IsConditionCodeInCorrectFormat(this string value)
        {
            Console.WriteLine("Checking format of ProviderSpecialty: " + value);
            return Regex.IsMatch(value, @"^[A-Z]{4}[ ][-][ ][A-Za-z\s]{1,}$");
        }
        ///<summary>
        /// XXXXX-XXXXX or XXXXX
        ///</summary>
        /// <param name="value">value to validate</param>
        /// <returns>boolean for whether format is correct or not</returns>
        public static bool IsProcCodeInCorrectFormatForSingleDoubleRange(this string value)
        {
            Console.WriteLine("Checking format of ProviderSpecialty: " + value);
            return Regex.IsMatch(value, @"^[a-zA-Z0-9]+-[a-zA-Z0-9]+$|^[a-zA-Z0-9]+$");
        } 
        ///<summary>
        /// XXXXX-XXXXX
        ///</summary>
        /// <param name="value">value to validate</param>
        /// <returns>boolean for whether format is correct or not</returns>
        public static bool IsProcCodeInCorrectFormatForRange(this string value)
        {
            Console.WriteLine("Checking format of ProviderSpecialty: " + value);
            return Regex.IsMatch(value, @"^[a-zA-Z0-9]{1,5}-[a-zA-Z0-9]{1,5}$");
        } ///<summary>
        /// XXXXX-XXXXX or null or XXXXX
        ///</summary>
        /// <param name="value">value to validate</param>
        /// <returns>boolean for whether format is correct or not</returns>
        public static bool IsTrigCodeEitherNullOrInCorrectFormat(this string value)
        {
            Console.WriteLine("Checking format of ProviderSpecialty: " + value);
            return Regex.IsMatch(value, @"^[a-zA-Z0-9]{1,5}-[a-zA-Z0-9]{1,5}$|^[a-zA-Z0-9]+$");
        }

        public static bool IsPhoneNumberInCorrectFormat(this string value)
        {
            Console.WriteLine("Checking format of phone Number: " + value);
            return Regex.IsMatch(value, @"^\(\d{3}\)\s\d{3}-\d{4}$");
        }


        public static bool IsFlagInCorrectFormat(this string value)
        {
            Console.WriteLine("Checking format of FLags: " + value);
            return  Regex.IsMatch(value, @"[A-Z]{3}(\,[ ][A-Z]{3})*");
        }


    }
}
