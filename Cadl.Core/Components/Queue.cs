using System;
namespace Cadl.Core.Components
{
    public class Queue : Component
    {
        public string QueueName { get; set; }
        public string StorageAccount { get; set; }
        public string ConnectionString { get; set; }
    }
}
