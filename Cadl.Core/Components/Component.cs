using System;
using System.Collections.Generic;

namespace Cadl.Core.Components
{
    public class Component
    {
        public Dictionary<string, string> Specs { get; set; } = new Dictionary<string, string>();
        public string ComponentName { get; set; }
    }
}
