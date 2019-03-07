using System;
using System.Collections.Generic;
using Cloudform.Core.Components;

namespace Cloudform.Core.Parsers
{
    public class QueueParser : ComponentParser
    {
        public override Component Parse(List<Line> lines, out int index)
        {
            index = 0;
            var queue = new Queue();

            if (!lines[0].PartsEqualTo(4))
            {
                throw new ParsingException(new Error(Error.UnknownSyntax));
            }
            else
            {
                queue.QueueName = lines[0].Parts[2];
                queue.ComponentName = lines[0].Parts[3];

                if (!Validator.ValidateComponentName(queue.QueueName))
                {
                    throw new ParsingException(new Error(Error.InvalidComponentName));
                }
            }

            return queue;;
        }
    }
}
