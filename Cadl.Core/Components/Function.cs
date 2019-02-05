using System;
using System.Collections.Generic;
using Cadl.Core.Parsers;

namespace Cadl.Core.Components
{
    public enum Trigger
    {
        Input,
        Queue,
        Timer
    }
    public class Function : Component
    {
        public string FunctionName { get; set; }
        public Trigger Trigger { get; set; }
        public string TriggeringQueue { get; set; }
        public string TriggeringMessage { get; set; }
        public int TriggeringTimeSecs { get; set; }
        public List<Line> Code { get; set; } = new List<Line>();
        public string Size { get; set; }
    }
}
