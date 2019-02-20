using System.Linq;
using System.Collections.Generic;
using System.Text;
using Cadl.Core.Parsers;
using Cadl.Core.Code;
using Cadl.Core.Components;
using Cadl.Core.Code.Azure.QueueSegments;
using System;

namespace Cadl.Core.Interpreters
{
    public class CadlInterpreter
    {
        private const int indentationUnit = 3;
        private int methodCount;
        private List<Segment> segments = new List<Segment>();
        private List<Component> components;
        private Dictionary<string, object> props;
        private Function function;
        private int indentCount = 1;

        public List<string> DependsOnModules { get; set; } = new List<string>();

        public string CompileToJs(Function function, List<Component> components, Dictionary<string, object> props)
        {
            this.function = function;
            this.components = components;
            var cadl = function.Code;
            this.props = props;

            segments.Add(new HelperSegment(indentCount));

            for (int i = 0; i < cadl.Count; i++)
            {
                var line = cadl[i];
                if (line.KeyExists("call"))
                {
                    //Todo
                }
                else if (line.KeyExists("sql"))
                {
                    GetScope(cadl.Skip(i).ToList(), out int jump);
                    segments.Add(SqlInterpreter.GetSqlSegement(components,
                        cadl.Skip(i).Take(jump).ToList(), indentCount, ref methodCount));
                    i += jump;
                }
                else if (line.KeyExists("enqueue"))
                {
                    segments.Add(GetEnqueueSegement(cadl[i]));

                }
                else if (line.KeyExists("dequeue"))
                {
                    segments.Add(GetDequeueSegement(cadl[i]));
                }
                else if (line.KeyExists("iterate"))
                {
                    segments.Add(new IterationSegment(indentCount,
                        GetScope(cadl.Skip(i).ToList(), out int x)));
                }
                else if (line.KeyExists("code"))
                {
                    segments.Add(GetCodeSegment(line));
                }
                else
                {
                    if (line.Content.IndexOf('{') != -1)
                    {
                        segments.Add(new JavaScriptSegment(indentCount, line.Content));
                        indentCount++;
                    }
                    else if (line.Content.IndexOf('}') != -1)
                    {
                        indentCount--;
                        segments.Add(new JavaScriptSegment(indentCount, line.Content));
                    }
                    else
                    {
                        segments.Add(new JavaScriptSegment(indentCount, line.Content));
                    }
                }
            }

            return Js();
        }

        private Segment GetCodeSegment(Line line)
        {
            if (line.PartsEqualTo(3))
            {
                return new CodeSegment(indentCount, line.Parts[1], line.Parts[2]);
            }
            else if (line.PartsEqualTo(5))
            {
                return new CodeSegment(indentCount, line.Parts[3], line.Parts[4], line.Parts[0]);
            }
            else
            {
                throw new ParsingException(new Error(Error.UnknownSyntax));
            }
        }

        private Segment GetEnqueueSegement(Line line)
        {
            if (!line.PartsEqualTo(3))
            {
                throw new ParsingException(new Error(Error.UnknownSyntax));
            }
            else
            {
                var queueName = line.Parts[1];
                var variable = line.Parts[2];
                var queue = components.OfType<Queue>().First(q => q.ComponentName == queueName);
                return new EnqueueSeqment(indentCount, queue, variable);
            }
        }

        private Segment GetDequeueSegement(Line line)
        {
            if (!line.PartsEqualTo(4))
            {
                throw new ParsingException(new Error(Error.UnknownSyntax));
            }
            else
            {
                var queueName = line.Parts[3];
                var variable = line.Parts[0];

                var queue = components.OfType<Queue>().FirstOrDefault(q => q.ComponentName == queueName);
                if (queue == null)
                {
                    throw new ParsingException(new Error(Error.UknownQueue, queueName));
                }
                return new DequeueSeqment(indentCount, queue, variable);
            }
        }

        private string CompileCall(Line line)
        {
            return null;
        }

        private string Js()
        {
            var requires = new List<string>();
            var globalVars = new List<string>();
            var methods = new List<string>();

            var dependencies = segments.SelectMany(s => s.DependsOnSegments).GroupBy(d => d.Name);

            foreach (var dependency in dependencies)
            {
                segments.Add(dependency.First());
            }

            foreach (var segment in segments)
            {
                foreach (var require in segment.Requires)
                {
                    if (!requires.Contains(require))
                    {
                        requires.Add(require);
                    }
                }

                foreach (var globalVar in segment.GlobalVars)
                {
                    globalVars.Add(globalVar);
                }

                foreach (var method in segment.Methods)
                {
                    methods.Add(method);
                }

                foreach (var module in segment.DependsOnModules)
                {
                    if (!DependsOnModules.Contains(module))
                    {
                        DependsOnModules.Add(module);
                    }
                }
            }

            var js = new StringBuilder();

            foreach (var require in requires)
            {
                js.Append(require);
                js.AppendLine();
            }

            js.AppendLine();

            foreach (var globalVar in globalVars)
            {
                js.Append(globalVar);
                js.AppendLine();
            }

            js.AppendLine();

            foreach (var method in methods)
            {
                js.Append(method);
                js.AppendLine();
            }

            js.AppendLine();

            BeginFunction(js);

            foreach (var segment in segments)
            {
                if (segment.FunctionCode != null)
                {
                    js.Append(Indent(segment.FunctionCode, segment.IndentCount * indentationUnit));
                    js.AppendLine();
                }
            }

            EndFunction(js);
            return js.ToString();
        }

        private string Indent(string text, int spaceCount)
        {
            var spaces = "";
            for (int i = 0; i <= spaceCount; i++)
            {
                spaces += ' ';
            }

            return spaces + text;
        }

        private List<Line> GetScope(List<Line> cadl, out int jump)
        {
            jump = 0;
            int scopeDepth = 0;

            for (jump = 0; jump < cadl.Count; jump++)
            {
                var line = cadl[jump];

                if (jump == 1)
                {
                    if (line.EnsureBeginScope()) ;
                }

                if (line.Parts[0].IndexOf('{') != -1)
                {
                    scopeDepth++;
                }
                else if (line.Parts[0].IndexOf('}') != -1)
                {
                    scopeDepth--;

                    if (scopeDepth < 0)
                    {
                        throw new ParsingException(new Error(Error.MissingOpenBrace));
                    }
                    else if (scopeDepth == 0)
                    {
                        break;
                    }
                }
            }

            return cadl.Take(jump).ToList();
        }

        private void BeginFunction(StringBuilder sb)
        {
            var argName = "req";
            if (function.Trigger == Trigger.Timer)
            {
                argName = "timer";
            }
            else if (function.Trigger == Trigger.Queue)
            {
                argName = function.InputMessage;
            }

            sb.Append($"module.exports = async function (context, {argName}) {{");
            sb.AppendLine();
            if (function.Trigger == Trigger.Request)
            {
                sb.Append(Indent("if (!req || !req.body) {", indentationUnit));
                sb.AppendLine();
                sb.Append(Indent($"'return {function.InputMessage} missing'", indentationUnit));
                sb.AppendLine();
                sb.Append(Indent("}", indentationUnit));
                sb.AppendLine();
                sb.Append(Indent($"var {function.InputMessage} = req.body;", indentationUnit));
                sb.AppendLine();
            }
        }

        private void EndFunction(StringBuilder sb)
        {
            sb.Append("}");
        }
    }
}
