using System;

namespace Cloudform.Core.Reporting
{
    public interface IEventLogger
    {
        void Log(int buildId, string eventDescription);
        void NextLine(int buildId);
    }
}