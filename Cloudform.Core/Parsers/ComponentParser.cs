using System;
using System.Linq;
using System.Collections.Generic;
using Cloudform.Core.Components;

namespace Cloudform.Core.Parsers
{
    public abstract class ComponentParser
    {
        public abstract Component Parse(List<Line> lines, out int index);
    }
}
