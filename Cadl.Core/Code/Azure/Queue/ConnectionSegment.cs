using System;
namespace Cadl.Core.Code.Azure.Queue
{
    public class ConnectionSegment : CodeSegment
    {
        private const string connect = @"
function connectTo#queue() {
    queueSvc.createQueueIfNotExists('#queue', function(error, results, response){
      if(!error){
        // Queue created or exists
      }
    });
}";
        public ConnectionSegment()
        {
            Requires.Add("var azure = require('azure-storage');");
        }
    }
}
