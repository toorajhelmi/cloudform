using System;
namespace Cadl.Core.Parsers
{
    public class Error
    {
        public const string ClosingBraceMissing = "The closing brace could not be found";
        public const string UnknownSyntax = "Unknown syntax";
        public const string UnknownType = "Unknown type";
        public const string MissingOpenBrace = "Missing {";
        public const string MissingCloseBrace = "Missing }";
        public const string UnknownComponent = "Unknown component";
        public const string TriggerExpected = "Tigger expected";
        public const string InvalidTimerPeriod = "Invalid timer period. Should be like days:hours:mins:secs";

        public Error(string message, int lineNumber = 0, string what = null)
        {
            LineNumber = lineNumber;
            Message = message;
            What = what; 
        }

        public int LineNumber { get; set; }
        public string Message { get; set; }
        public string What { get; set; }
    }
}
