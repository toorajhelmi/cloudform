using System;
using System.Collections.Generic;
using Cadl.Core.Arctifact;
using Cadl.Core.Components;
using Cadl.Core.Extensions;
using Cadl.Core.Interpreters.Messages;

namespace Cadl.Core.Interpreters
{
    public abstract class Interpreter
    {
        protected Dictionary<string, object> props;
        protected Factory factory;

        public Interpreter(Factory factory, Dictionary<string, object> config)
        {
            props = config.Copy();
            this.factory = factory;
        }

        public abstract void Interpret();
    }
}
