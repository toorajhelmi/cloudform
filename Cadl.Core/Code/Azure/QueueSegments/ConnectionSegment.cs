using System;
using Cadl.Core.Components;

namespace Cadl.Core.Code.Azure.QueueSegments
{
    public class ConnectionSegment : Segment
    {
        private const string connect = @"
function connectTo#storage-account() {
    if (#storage-account_queue != null)
    {
        #storage-account_queue = azure.createQueueService(#connection-string);
    }
}";
        public ConnectionSegment(int indentCount, Queue queue)
            : base(indentCount)
        {
            Name = $"{queue.StorageAccount}_queue";
            Requires.Add("var azure = require('azure-storage');");
            GlobalVars.Add($"var {queue.StorageAccount}_queue;");

            Methods.Add(connect
                .Replace("#storage-account", queue.StorageAccount)
                .Replace("#connection-string", queue.ConnectionString)
            );
        }
    }
}
