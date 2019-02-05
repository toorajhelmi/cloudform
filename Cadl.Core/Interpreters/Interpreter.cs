using System;
using System.Collections.Generic;
using System.IO;
using Cadl.Core.Interpreters.Azure;
using Cadl.Core.Components;
using Cadl.Core.Interpreters.Messages;

namespace Cadl.Core.Interpreters
{
    public enum TargetCloud
    {
        Azure,
        Aws,
        Gcp
    }

    public abstract class Interpreter
    {
        protected Dictionary<string, string> config;

        public Interpreter(Dictionary<string, string> config)
        {
            this.config = config;
        }

        public abstract void Interpret(string outputPath, List<Component> components, List<Message> messages);
    }
}
