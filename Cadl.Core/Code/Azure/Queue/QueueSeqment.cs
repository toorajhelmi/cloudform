using System;
namespace Cadl.Core.Code.Azure
{
    public class QueueSeqment : CodeSegment
    {
        public QueueSeqment()
        {
            Requires.Add("var azure = require('azure-storage');");
        }
    }
}
