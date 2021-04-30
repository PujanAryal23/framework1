using System;

namespace Nucleus.Service.Support.Common.Constants
{
    public sealed class StringFormatter
    {
        public const string BreakLine = "******************************************************************************************************************************************************";
        public const string ServerError = "Server Error in '/' Application.";
        public const string ServerErrorXPath = "//body/span/h1";
        private const string AsterickLeftPad = "**************************************";

        public static void PrintMessageTitle(string msgTitle)
        {
            Console.Out.WriteLine((AsterickLeftPad + msgTitle).PadRight(BreakLine.Length, '*'));
        }

        public static void PrintLineBreak()
        {
            Console.Out.WriteLine(BreakLine);
        }

        public static void PrintMessage(string message)
        {
            Console.Out.WriteLine(message);
        }


    }
}
