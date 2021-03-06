﻿using System;
using System.Linq;
using System.Collections.Generic;
using Cloudform.Core.Components;
using Cloudform.Core.Specs;
using Cloudform.Core.Extensions;

namespace Cloudform.Core.Parsers
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
                    SetInAndOut(line);
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
                    if (line.Content.Contains("return"))
                    {
                        function.Returns = true;
                    }

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

        private void SetInAndOut(Line line)
        {
            if (!line.PartsMoreThan(2))
            {
                throw new ParsingException(new Error(Error.UnknownSyntax));
            }
            else if (line.Parts[0] != "input")
            {
                throw new ParsingException(new Error(Error.TriggerExpected));
            }
            else
            {
                switch (line.Parts[1])
                {
                    case "request":
                        function.Trigger = Trigger.Request;
                        function.InputMessage = line.Parts[2];
                        break;
                    case "queue":
                        function.Trigger = Trigger.Queue;
                        function.InputMessage = line.Parts[3];
                        function.InputQueueName = line.Parts[2];
                        break;
                    case "timer":
                        function.Trigger = Trigger.Timer;
                        var periodParts = line.Parts[2].Split(new[] { ':' });
                        if (int.TryParse(line.Parts[2], out int periodSecs))
                        {
                            function.PeriodSecs = periodSecs;
                        }
                        else
                        {
                            throw new ParsingException(new Error(Error.InvalidTimerPeriod));
                        }
                        //function.TriggeringTimeSecs = ((int.Parse(periodParts[0]) * 24
                        //+ int.Parse(periodParts[1])) * 60
                        //+ int.Parse(periodParts[2])) * 60
                        //+ int.Parse(periodParts[3]);
                        break;
                }

                if (line.KeyExists("output"))
                {
                    function.OutputQueueName = line.Parts[line.Parts.IndexOf("output") + 1];
                }
            }
        }

    }
}
