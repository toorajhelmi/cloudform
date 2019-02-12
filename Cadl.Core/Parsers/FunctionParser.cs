using System;
using System.Linq;
using System.Collections.Generic;
using Cadl.Core.Components;
using Cadl.Core.Specs;
using Cadl.Core.Extensions;

namespace Cadl.Core.Parsers
{
    public class FunctionParser : ComponentParser
    {
        private Function function;
        private bool openBraceFound;
        private int scopeDepth;

        public override Component Parse(List<Line> lines, out int index)
        {
            function = new Function();
            for (index = 0; index < lines.Count(); index++)
            {
                var line = lines[index];
                if (index == 0)
                {
                    SetFunctionProperties(line);
                }

                else if (index == 1)
                {
                    SetTrigger(line);
                }

                else if (index == 2)
                {
                    if (line.Parts[0].IndexOf('{') == -1)
                    {
                        throw new ParsingException(new Error(Error.MissingOpenBrace));
                    }
                    else
                    {
                        scopeDepth++;
                        openBraceFound = true;
                    }
                }

                else if (line.Parts[0].IndexOf('{') != -1)
                {
                    scopeDepth++;
                    function.Code.Add(line);
                }

                else if (line.Parts[0].IndexOf('}') != -1)
                {
                    if (!openBraceFound)
                    {
                        throw new ParsingException(new Error(Error.MissingOpenBrace));
                    }
                    else
                    {
                        scopeDepth--;
                        if (scopeDepth == 0)
                        {
                            return function;
                        }
                        else
                        {
                            function.Code.Add(line);
                        }
                    }
                }

                else
                {
                    function.Code.Add(line);
                }
            }

            throw new ParsingException(new Error(Error.MissingCloseBrace));
        }

        private void SetFunctionProperties(Line line)
        {
            if (!line.PartsMoreThan(3))
            {
                throw new ParsingException(new Error(Error.UnknownSyntax));
            }
            else
            {
                function.FunctionName = NameGenerator.Unique(line.Parts[2]);
                function.ComponentName = line.Parts[3];
                var sizeType = line.AtVal(4, "size") ?? "M";
                function.Size =  AzureDefinedSpecs.Instance.Values[$"Function.{sizeType}"]["size"];
            }
        }

        private void SetTrigger(Line line)
        {
            if (!line.PartsEqualTo(3))
            {
                throw new ParsingException(new Error(Error.UnknownSyntax));
            }
            else if (line.Parts[0] != "trigger")
            {
                throw new ParsingException(new Error(Error.TriggerExpected));
            }
            else
            {
                switch (line.Parts[1])
                {
                    case "request":
                        function.Trigger = Trigger.Request;
                        function.TriggeringMessage = line.Parts[2];
                        break;
                    case "queue":
                        function.Trigger = Trigger.Queue;
                        function.TriggeringQueue = line.Parts[2];
                        break;
                    case "timer":
                        function.Trigger = Trigger.Timer;
                        var periodParts = line.Parts[2].Split(new[] { ':' });
                        if (periodParts.Length != 4)
                        {
                            throw new ParsingException(new Error(Error.InvalidTimerPeriod));
                        }
                        function.TriggeringTimeSecs = ((int.Parse(periodParts[0]) * 24
                                                        + int.Parse(periodParts[1])) * 60
                                                        + int.Parse(periodParts[2])) * 60
                                                        + int.Parse(periodParts[3]);
                        break;
                }
            }
        }

    }
}
