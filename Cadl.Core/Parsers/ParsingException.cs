using System;
namespace Cadl.Core.Parsers
{
    public class ParsingException : Exception
    {
        public ParsingException(Error error)
        {
            Error = error;
        }

        public Error Error { get; set; }
    }
}
