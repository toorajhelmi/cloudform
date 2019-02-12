using System;
using Cadl.Core.Components;

namespace Cadl.Core.Code.Azure.QueueSegments
{
    public class ConnectionSegment : CodeSegment
    {
        private const string connect = @"
function connectTo#stoage-account() {
    if (#storage-account_queue != null)
    {
        #storage-account_queue = azure.createQueueService(#connection-string);
    }
}";
        public ConnectionSegment(Queue queue)
        {
            Name = $"{queue.StorageAccount}_queue";
            Requires.Add("var azure = require('azure-storage');");
            GlobalVars.Add($"{queue.StorageAccount}_queue;");

            Methods.Add(connect
                .Replace("#storage-account", queue.StorageAccount)
                .Replace("#connection-string", queue.ConnectionString)
            );
        }
    }
}
