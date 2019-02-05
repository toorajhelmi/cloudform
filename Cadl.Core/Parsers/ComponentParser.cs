using System;
using System.Linq;
using System.Collections.Generic;
using Cadl.Core.Components;

namespace Cadl.Core.Parsers
{
    public abstract class ComponentParser
    {
        public abstract Component Parse(List<Line> lines, out int index);
    }
}
