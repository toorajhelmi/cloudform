using System;
using Cadl.Core.Components;

namespace Cadl.Core.Code.Azure.QueueSegments
{
    public class DequeueSeqment : Segment
    {
        private const string queueMessage = @"
function dequeue_#queue()
{
    return new Promise(function (resolve, reject) {
        connectTo#storage-account();
        #storage-account_queue.getMessages('#queue', function(error, results, response) {
            if (!error) {
                var message = results[0];
                #storage-account_queue.deleteMessage('#queue', message.messageId, message.popReceipt, function(error, response) {
                    if (!error) {
                        resovle(message.messageText);
                    }
                    else
                    {
                        reject(error);
                    }
                });
            }
            else
            {
                reject(error);
            }
        });
    });
}";

        public DequeueSeqment(int indentCount, Queue queue, string output)
            :  base(indentCount)
        {
            Dependencies.Add(new ConnectionSegment(indentCount, queue));
            Methods.Add(queueMessage
                .Replace("#stoage-account", queue.StorageAccount)
                .Replace("#queue", queue.QueueName));
            FunctionCode = $"var {output} = await dequeue_{queue.QueueName};";
        }
    }
}
