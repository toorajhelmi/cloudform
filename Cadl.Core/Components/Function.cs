using System;
using System.Collections.Generic;
using Cloudform.Core.Parsers;

namespace Cloudform.Core.Components
{
    public enum Trigger
    {
        Request,
        Queue,
        Timer
    }
    public class Function : Component
    {
        public const string FolderName = "Function";
        public string FunctionName { get; set; }
        public List<Line> Code { get; set; } = new List<Line>();
        public string Size { get; set; }
        public bool Returns { get; set; }

        public Trigger Trigger { get; set; }

        //Only for Timer Trigger
        public int PeriodSecs { get; set; }

        //Only for Queue Trigger
        public string InputQueueName { get; set; }
        public Queue InputQueue { get; set; }
        public string InputMessage { get; set; }

        public string OutputQueueName { get; set; }
        public Queue OutputQueue { get; set; }
    }
}
