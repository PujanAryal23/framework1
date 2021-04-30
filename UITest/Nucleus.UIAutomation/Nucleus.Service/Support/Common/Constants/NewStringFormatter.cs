using System;

namespace Nucleus.Service.Support.Common.Constants
{
    public sealed class NewStringFormatter
    {
        public const string BreakLine = "******************************************************************************************************************************************************";
        public const string ServerError = "Server Error in '/' Application.";
        public const string ServerErrorXPath = "//body/span/h1";
        private const string AsterickLeftPad = "**************************************";

        public void PrintMessageTitle(string msgTitle)
        {
            Console.Out.WriteLine((AsterickLeftPad + msgTitle).PadRight(BreakLine.Length, '*'));
        }

        public void PrintLineBreak()
        {
            Console.Out.WriteLine(BreakLine);
        }

        public void PrintMessage(string message)
        {
            Console.Out.WriteLine(message);
        }


    }
}