using System;
using System.Linq;

namespace Cadl.Core.Parsers
{
    public class Validator
    {
        public static bool ValidateName(string name)
        {
            return name.All(c => !"{}[]()=,".Contains(c));
        }
    }
}
