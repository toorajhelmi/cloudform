using System;
using System.Collections.Generic;

namespace Cadl.Core.Code
{
    public class CodeSegment
    {
        public virtual string Name { get; protected set; }
        public List<string> Requires { get; set; } = new List<string>();
        public List<string> GlobalVars { get; set; } = new List<string>();
        public List<string> Methods { get; set; } = new List<string>();
        public List<CodeSegment> Dependencies { get; set; } = new List<CodeSegment>();
        public virtual string FunctionCode { get; set; }
    }
}
