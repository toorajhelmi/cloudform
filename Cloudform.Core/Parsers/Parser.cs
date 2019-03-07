using System;
using System.Collections.Generic;
using System.Linq;
using Cloudform.Core.Arctifact;
using Cloudform.Core.Components;

namespace Cloudform.Core.Parsers
{
    public class Parser
    {
        protected enum Scope
        {
            None,
            Message,
            Component
        }

        private int index;
        protected List<Line> lines = new List<Line>();
        protected Factory factory;

        public void Parse(Factory factory)
        {
            var messageParser = new MessageParser();

            this.factory = factory;
            ConvertToLines();

            for (index = 0; index < lines.Count; index++)
            {
                try
                {
                    var line = lines[index];

                    if (line.Parts[0] == "message")
                    {
                        var message = messageParser.Parse(lines.Skip(index).ToList(), out int moveAhead);
                        Console.WriteLine($"Parsed message: {message.Name}");
                        factory.Messages.Add(message);
                        index += moveAhead;
                    }
                    else if (line.Parts[0] == "component")
                    {
                        var componentParser = SelectComponentParser(lines[index]);
                        var component = componentParser.Parse(lines.Skip(index).ToList(), out int moveAhead);
                        Console.WriteLine($"Parsed component: [{component.GetType().Name}] {component.ComponentName}");
                        factory.Components.Add(component);
                        index += moveAhead;
                    }
                    else
                    {
                        throw new ParsingException(new Error(Error.UnknownSyntax, index));
                    }
                }
                catch (ParsingException pe)
                {
                    pe.Error.LineNumber = index;
                    throw pe;
                }
            }

            MapFunctionsToQueues();
            ParseCloudSpecific();
        }

        protected virtual void ParseCloudSpecific()
        {
        }

        private void MapFunctionsToQueues()
        {
            var queues = factory.Components.OfType<Queue>();
            foreach (var function in factory.Components.OfType<Function>())
            {
                if (function.Trigger == Trigger.Queue)
                {
                    function.InputQueue = queues.First(q => q.ComponentName == function.InputQueueName);
                }
                if (function.OutputQueueName != null)
                {
                    function.OutputQueue = queues.First(q => q.ComponentName == function.OutputQueueName);
                }
            }
        }

        private void ConvertToLines()
        {
            var lineTexts = factory.Script.Split(new char[] { '\n' }).ToList();
            for (int i = 0; i < lineTexts.Count; i++)
            {
                lineTexts[i] = lineTexts[i].Trim(new[] { ' ', '\n', '\r', '\t' });

                //Remove comments
                var index = lineTexts[i].IndexOf("//");
                if (index != -1)
                {
                    var comment = lineTexts[i].Substring(index, lineTexts[i].Length - index);
                    lineTexts[i] = lineTexts[i].Replace(comment, "").Trim();
                }

                if (lineTexts[i] != "")
                {
                    lines.Add(new Line(lineTexts[i]));
                }
            }
        }

        private ComponentParser SelectComponentParser(Line line)
        {
            switch (line.Parts[1])
            {
                case "Function": return new FunctionParser();
                case "SQL": return new SqlParser();
                case "Queue": return new QueueParser();
                case "NoSql": return new NoSqlParser();
                default: throw new ParsingException(new Error(Error.UnknownComponent, index, line.Parts[1]));
            }
        }
    }
}
