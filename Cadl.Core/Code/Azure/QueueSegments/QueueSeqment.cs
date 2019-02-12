using System;
using Cadl.Core.Components;

namespace Cadl.Core.Code.Azure.QueueSegments
{
    public class QueueSeqment : CodeSegment
    {
        private const string queueMessage = @"
function queue_#queue()
{
    #queue_queue.createMessage(#queue, #message, function(error) {
        if (!error) {
            // Message inserted
        }
    });
}";

        public QueueSeqment(Queue queue, string message)
        {
            Dependencies.Add(new ConnectionSegment(queue));
            Methods.Add(queueMessage
                .Replace("#queue", queue.QueueName)
                .Replace("#queue", message));
        }
    }
}
