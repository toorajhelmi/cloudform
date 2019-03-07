using System;
namespace Cloudform.Core.Extensions
{
    public class NameGenerator
    {
        public static string Unique(string baseName, int? maxLength = null)
        {
            var unique = $"{baseName}{Guid.NewGuid().ToString().ToLower()}".Replace("-", "");
           
            if (maxLength != null)
            {
                return unique.Substring(0, maxLength.Value);
            }
            else
            {
                return unique;
            }
        }
    }
}
