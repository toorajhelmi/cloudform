using System;
using System.Linq;

namespace Cloudform.Core.Parsers
{
    public class Validator
    {
        public static bool ValidateName(string name)
        {
            return name.All(c => !"{}[]()=,".Contains(c));
        }

        public static bool ValidateComponentName(string name)
        {
            return name.All(c => Char.IsLetter(c) || Char.IsDigit(c) || c == '-') &&
                name == name.ToLower() && char.IsLetter(name[0]);
        }
    }
}
