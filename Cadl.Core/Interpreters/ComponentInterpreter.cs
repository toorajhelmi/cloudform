using System;
using System.Collections.Generic;
using Cadl.Core.Deployers;
using Cadl.Core.Components;

namespace Cadl.Core.Interpreters
{
    public abstract class ComponentInterpreter
    {
        public abstract void Interpret(string outputPath, Component component, Dictionary<string, string> config);
    }
}
