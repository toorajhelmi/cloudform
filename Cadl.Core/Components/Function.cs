using System;
using System.Collections.Generic;
using Cadl.Core.Parsers;

namespace Cadl.Core.Components
{
    public enum Trigger
    {
        Request,
        Queue,
        Timer
    }
    public class Function : Component
    {
        public string FunctionName { get; set; }
        public List<Line> Code { get; set; } = new List<Line>();
        public string Size { get; set; }
        public bool Returns { get; set; }

        public Trigger Trigger { get; set; }

        //Only for Timer Trigger
        public int PeriodSecs { get; set; }

        //Only for Queue Trigger
        public string TriggeringQueueName { get; set; }
        public Queue TriggeringQueue { get; set; }
        public string TriggeringMessage { get; set; }
    }
}
