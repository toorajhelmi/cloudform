using System.Linq;
using System.Collections.Generic;
using Cloudform.Core.Parsers;

namespace Cloudform.Core.Components
{
    public class Table
    {
        private string[] types = { "int", "decimal", "datetime", "bit", "char", "varchar", "bianry", "uniqueidentifier" };

        public string Name { get; set; }
        public Dictionary<string, string> Columns { get; set; } = new Dictionary<string, string>();
        public List<string[]> InsertValues { get; set; } = new List<string[]>();
        public string TSql { get; set; }
        public string Inserts { get; set; }
        
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

        public static string ToTediousTypes(string sqlType)
        {
            switch (sqlType.ToLower())
            {
                case "int": return "TYPES.Int";
                case "decimal": return "TYPES.Decimal";
                case "datetime": return "TYPES.Datetime";
                case "bit": return "TYPES.Bit";
                case "char": return "TYPES.Char";
                case "varchar": return "TYPES.VarChar";
                case "bianry": return "TYPES.Bianry";
                case "uniqueidentifier": return "TYPES.UniqueIdentifier";
                default: throw new ParsingException(new Error(Error.UnknownType, sqlType));

            }
        }
    }
}
