using System;
namespace Cadl.Core.Code
{
    public class JavaScriptSegment : Segment
    {
        public JavaScriptSegment(int indentCount, string code)
            : base(indentCount)
        {
            FunctionCode = code;
        }
    }
}
