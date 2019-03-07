using System;
namespace Cloudform.Core.Code
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
