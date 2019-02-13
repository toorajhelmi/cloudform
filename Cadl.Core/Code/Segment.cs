using System;
using System.Collections.Generic;

namespace Cadl.Core.Code
{
    public class Segment
    {
        public Segment(int indentCount)
        {
            IndentCount = indentCount;
        }

        public virtual string Name { get; protected set; }
        public List<string> Requires { get; set; } = new List<string>();
        public List<string> GlobalVars { get; set; } = new List<string>();
        public List<string> Methods { get; set; } = new List<string>();
        public List<Segment> Dependencies { get; set; } = new List<Segment>();
        public virtual string FunctionCode { get; set; }
        public int IndentCount { get; set; }
    }
}
