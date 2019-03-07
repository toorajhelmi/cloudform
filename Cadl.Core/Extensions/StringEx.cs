using System.Linq;

namespace Cloudform.Core.Extensions
{
    public static class StringEx
    {
        public static string CleanFrom(this string str, char[] chars)
        {
            var output = "";
            foreach (var c in str)
            {
                if (!chars.Any(ch => ch == c))
                {
                    output += c;

                }
            }

            return output;
        }
    }
}
