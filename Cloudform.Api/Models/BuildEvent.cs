using System;
namespace Cloudform.Api.Models
{
    public class BuildEvent
    {
        public int BuildId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Event { get; set; }
    }
}
