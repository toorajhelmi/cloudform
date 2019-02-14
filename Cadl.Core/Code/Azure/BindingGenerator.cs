using System;
using System.Text;
using Cadl.Core.Components;
using Cadl.Core.Parsers;

namespace Cadl.Core.Code.Azure
{
    public class BindingGenerator
    {
        private const string header = @"
{
    ""bindings"": [";

        private const string trailer = @"
    ]
}";

        private const string requestTrigger = @"
    {
        ""type"": ""httpTrigger"",
        ""direction"": ""in"",
        ""name"": ""req"",
        ""methods"": [
        ""get"",
        ""post""
        ]
    }";

        private const string queueTrigger = @"
    {
        ""name"": ""#assign-to"",
        ""type"": ""queueTrigger"",
        ""direction"": ""in"",
        ""queueName"": ""#queue-name"",
        ""connection"": ""#connection-string""
    }";

        private const string timerTrigger = @"
    {
        ""schedule"": ""#schedule"",
        ""name"": ""#timer"",
        ""type"": ""timerTrigger"",
        ""direction"": ""in""
    }";

        private const string outputBinding = @"
    {
        ""type"": ""http"",
        ""direction"": ""out"",
        ""name"": ""$return""
    }";

        public BindingGenerator(Function function)
        {
            switch (function.Trigger)
            {
                case Trigger.Queue:
                    var queue = function.TriggeringQueue;
                    Set(function.TriggeringMessage, queue.QueueName, 
                        queue.StorageAccount, function.Returns);
                    break;
                case Trigger.Request:
                    Set(function.Returns);
                    break;
                case Trigger.Timer:
                    Set(function.PeriodSecs, function.Returns);
                    break;
            }
        }

        public string Bindings { get; private set; }

        //Request Trigger
        private void Set(bool returns)
        {
            var sb = new StringBuilder();
            sb.Append(header);
            sb.Append(requestTrigger);

            if (returns)
            {
                sb.Append(",");
                sb.Append(outputBinding);
            }

            sb.Append(trailer);
            Bindings = sb.ToString().Trim();
        }

        //Queue Trigger
        private void Set(string assignTo, string queueName, string connectionString, bool returns)
        {
            var sb = new StringBuilder();
            sb.Append(header);
            sb.Append(queueTrigger.Replace("#assign-to", assignTo)
                .Replace("#queue-name", queueName)
                .Replace("#connection-string", connectionString));

            if (returns)
            {
                sb.Append(",");
                sb.Append(outputBinding);
            }

            sb.Append(trailer);
            Bindings = sb.ToString();
        }

        //Timer Trigger
        private void Set(int periodSec, bool returns)
        {
            var hours = periodSec / 3600;
            var mins = (periodSec - hours * 3600) / 60;
            var secs = periodSec - hours * 3600 - mins * 60;

            var schedule = new [] {"*", "*", "*", "*", "*", "*"};
            if (hours != 0)
            {
                schedule[2] = $"*/{hours}";
                schedule[1] = "0";
                schedule[0] = "0";
            }
            else if (mins != 0)
            {
                schedule[1] = $"*/{mins}";
                schedule[0] = "0";
            }
            else if (secs != 0)
            {
                schedule[0] = $"*/{secs}";
            }
            else
            {
                throw new ParsingException(new Error(Error.InvalidTimerPeriod));
            }

            var sb = new StringBuilder();
            sb.Append(header);
            sb.Append(timerTrigger).Replace("#timer", "timer")
                .Replace("#schedule", string.Join(' ', schedule));

            if (returns)
            {
                sb.Append(",");
                sb.Append(outputBinding);
            }

            sb.Append(trailer);
            Bindings = sb.ToString();
        }
    }
}
