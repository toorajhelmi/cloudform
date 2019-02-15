using System;
using System.Linq;
using System.Collections.Generic;
using Cadl.Core.Parsers;
using Cadl.Core.Extensions;

namespace Cadl.Core.Code
{
    public class IterationSegment : Segment
    {
        public IterationSegment(int indentCount, List<Line> scope)
            : base(indentCount)
        {
            var isAsync = false;
            foreach (var line in scope)
            {
                if (line.Content.Contains("sql") || line.Content.Contains("await"))
                {
                    isAsync = true;
                    break;
                }
            }

            if (!isAsync)
            {
                FunctionCode = scope[0].Content.Replace("iterate", "forEach");
            }
            else
            {
                //Convert from expiredHolds.iterate((expiredHold) =>
                //to await iterate(expiredHolds, async (expiredHold) =>
                var listVar = scope[0].Content.Split('.')[0];
                var iterationVar = scope[0].Content.Split('(').Last()
                    .CleanFrom(new[] { '=', '>', ')' }).Trim();
                FunctionCode = $"await iterate({listVar}, async ({iterationVar}) =>"; 
            }
        }
    }
}
