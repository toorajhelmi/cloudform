using System;
using System.Collections.Generic;
using Cadl.Core.Components;

namespace Cadl.Core.Parsers
{
    public class NoSqlParser : ComponentParser
    {
        public override Component Parse(List<Line> lines, out int index)
        {
            index = 0;
            return new NoSql();
        }
    }
}
