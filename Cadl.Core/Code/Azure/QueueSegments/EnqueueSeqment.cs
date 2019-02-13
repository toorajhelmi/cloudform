﻿using System;
using Cadl.Core.Components;

namespace Cadl.Core.Code.Azure.QueueSegments
{
    public class EnqueueSeqment : Segment
    {
        private const string queueMessage = @"
function enqueue_#queue(#message)
{
    connectTo#stoage-account();
    #queue_queue.createMessage('#queue', #message, function(error) {
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
                .Replace("#stoage-account", queue.StorageAccount)
                .Replace("#queue", queue.QueueName)
                .Replace("#message", message));
            FunctionCode = $"enqueue_{queue.QueueName}({message});";
        }
    }
}