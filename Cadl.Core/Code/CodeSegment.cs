using System;
using System.Collections.Generic;

namespace Cloudform.Core.Code
{
    public class CodeSegment : Segment
    {
        private const string function = @"
//This method should be completed with your custom logic 
function #method-name(#args)
{
    //Add you logic here
    //#return
}";

        public CodeSegment(int indentCount, string methodName, string args, string output = null)
            : base(indentCount)
        {
            if (output != null)
            {
                Methods.Add(function
                    .Replace("#method-name", methodName)
                    .Replace("#args", args)
                    .Replace("#return", $"Should return a value to be assinged to '{output}'"));
                FunctionCode = $"var {output} = {methodName}({args});";
            }
            else
            {
                Methods.Add(function.Replace("#method-name", methodName)
                        .Replace("#args", args)
                        .Replace("#return", ""));
                FunctionCode = $"{methodName}({args});";
            }
        }
    }
}
