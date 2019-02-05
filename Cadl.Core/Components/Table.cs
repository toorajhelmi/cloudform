using System.Linq;
using System.Collections.Generic;
using Cadl.Core.Parsers;

namespace Cadl.Core.Components
{
    public class Table
    {
        private string[] types = { "int", "string", "datetime" };

        public string Name { get; set; }
        public Dictionary<string, string> Columns { get; set; } = new Dictionary<string, string>();
        public List<string[]> InsertValues { get; set; } = new List<string[]>();
        public string TSql { get; set; }
        public string Insests { get; set; }
        
        public void AddColumn(string name, string type)
        {
            if (types.Any(t => t == type.ToLower()))
            {
                Columns.Add(name, type.ToLower());
            }
            else
            {
                throw new ParsingException(new Error(Error.UnknownType, 0, type));
            }
        }
    }
}
