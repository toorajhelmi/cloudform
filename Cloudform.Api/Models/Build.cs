using System;
namespace Cloudform.Api.Models
{
    public class Build
    {
        public int BuildId { get; set; }
        public int EventId { get; set; }
        public string Event { get; set; }
    }
}
