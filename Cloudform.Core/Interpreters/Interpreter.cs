using System;
using System.Collections.Generic;
using Cloudform.Core.Arctifact;
using Cloudform.Core.Components;
using Cloudform.Core.Extensions;
using Cloudform.Core.Interpreters.Messages;

namespace Cloudform.Core.Interpreters
{
    public abstract class Interpreter
    {
        protected Dictionary<string, object> props;
        protected Factory factory;

        public Interpreter(Factory factory)
        {
            props = factory.Props.Copy();
            this.factory = factory;
        }

        public abstract void Interpret();
    }
}
