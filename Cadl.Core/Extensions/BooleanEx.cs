using System;
namespace Cadl.Core.Extensions
{
    public static class BooleanEx
    {
        public static string ToLowerString(this bool value)
        {
            return value ? "true" : "false";
        }
    }
}
