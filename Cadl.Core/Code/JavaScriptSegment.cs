using System;
namespace Cadl.Core.Code
{
    public class JavaScriptSegment : CodeSegment
    {
        public JavaScriptSegment(string code)
        {
            FunctionCode = code;
        }
    }
}
