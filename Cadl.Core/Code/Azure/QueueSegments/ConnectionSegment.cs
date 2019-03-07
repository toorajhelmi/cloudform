using System;
using Cloudform.Core.Components;

namespace Cloudform.Core.Code.Azure.QueueSegments
{
    public class ConnectionSegment : Segment
    {
        private const string connect = @"
function connectTo#storage-account() {
    if (#storage-account_queue == null)
    {
        #storage-account_queue = azure.createQueueService(#connection-string);
        qsae422502e984a466089992_queue.messageEncoder = null;
    }
}";
        public ConnectionSegment(int indentCount, Queue queue)
            : base(indentCount)
        {
            Name = $"{queue.StorageAccount}_queue";
            Requires.Add("var azure = require('azure-storage');");
            DependsOnModules.Add("\"azure-storage\": \"2.10.2\"");
            GlobalVars.Add($"var {queue.StorageAccount}_queue;");

            Methods.Add(connect
                .Replace("#storage-account", queue.StorageAccount)
                .Replace("#connection-string", queue.ConnectionString)
            );
        }
    }
}
