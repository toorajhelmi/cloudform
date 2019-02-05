using System;
using System.Linq;
using System.Collections.Generic;
using Cadl.Core.Parsers;

namespace Cadl.Core.Interpreters.Messages
{
    public class Message
    {
        private string[] types = { "int", "string", "datetime" };

        public string Name { get; set; }
        public Dictionary<string, string> Fields { get; private set; } = new Dictionary<string, string>();

        public void AddField(string name, string type)
        {
            if (types.Any(t => t == type.ToLower()))
            {
                Fields.Add(name, type.ToLower());
            }
            else
            {
                throw new ParsingException(new Error(Error.UnknownType, 0, type));
            }
        }
    }
}
