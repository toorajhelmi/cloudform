using System;
using Cloudform.Core.Reporting;

namespace Cloudform.Cli
{
    public class EventLogger : IEventLogger
    {
        private int eventId = 0;
        public void Log(int buildId, string eventDescription)
        {
            Console.WriteLine($"{eventId++}, {eventDescription}");
        }

        public void NextLine(int buildId)
        {
            Console.WriteLine($"{eventId++}");
        }
    }
}
