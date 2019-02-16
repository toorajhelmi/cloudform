using System;
using Cadl.Core.Components;

namespace Cadl.Core.Code.Azure.QueueSegments
{
    public class EnqueueSeqment : Segment
    {
        private const string queueMessage = @"
function enqueue_#queue(#message)
{
    connectTo#storage-account();
    #storage-account_queue.createMessage('#queue', JSON.stringify(#message), function(error) {
        if (!error) {
            // Message inserted
        }
    });
}";

        public EnqueueSeqment(int indentCount, Queue queue, string message)
            : base(indentCount)
        {
            Dependencies.Add(new ConnectionSegment(indentCount, queue));
            Methods.Add(queueMessage
                .Replace("#storage-account", queue.StorageAccount)
                .Replace("#queue", queue.QueueName)
                .Replace("#message", message));
            FunctionCode = $"enqueue_{queue.QueueName}({message});";
        }
    }
}
