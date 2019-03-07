using System;
namespace Cloudform.Core.Extensions
{
    public static class BooleanEx
    {
        public static string ToLowerString(this bool value)
        {
            return value ? "true" : "false";
        }
    }
}
