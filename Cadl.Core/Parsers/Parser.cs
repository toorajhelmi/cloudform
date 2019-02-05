using System;
using System.Linq;
using System.Collections.Generic;
using Cadl.Core.Interpreters.Messages;
using Cadl.Core.Components;

namespace Cadl.Core.Parsers
{
    public class Parser
    {
        protected enum Scope
        {
            None,
            Message,
            Component
        }

        private List<Line> lines = new List<Line>();
        private string script;
        private int index;

        public List<Component> Components { get; set; } = new List<Component>();
        public List<Message> Messages { get; set; } = new List<Message>();

        public void Parse(string script)
        {
            var messageParser = new MessageParser();

            this.script = script;
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
                        Messages.Add(message);
                        index += moveAhead;
                    }
                    else if (line.Parts[0] == "component")
                    {
                        var componentParser = SelectComponentParser(lines[index]);
                        var component = componentParser.Parse(lines.Skip(index).ToList(), out int moveAhead);
                        Console.WriteLine($"Parsed component: [{component.GetType().Name}] {component.ComponentName}");
                        Components.Add(component);
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
        }

        private void ConvertToLines()
        {
            var lineTexts = script.Split(new char[] { '\n' }).ToList();
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
