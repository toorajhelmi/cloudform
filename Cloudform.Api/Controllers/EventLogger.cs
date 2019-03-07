using System;
using Cloudform.Api.Models;
using Cloudform.Core.Reporting;

namespace Cloudform.Api.Controllers
{
    public class EventLogger : IEventLogger
    {
        private int eventId = 0;

        public void Log(int buildId, string eventDescription)
        {
            using (var context = new CloudformContext())
            {
                context.Builds.Add(new Build
                {
                    BuildId = buildId,
                    EventId = eventId++,
                    Event = eventDescription
                });
                context.SaveChanges();
            }
        }

        public void NextLine(int buildId)
        {
            using (var context = new CloudformContext())
            {
                context.Builds.Add(new Build
                {
                    BuildId = buildId,
                    EventId = eventId++,
                    Event = "\n"
                });
                context.SaveChanges();
            }
        }
    }
}
