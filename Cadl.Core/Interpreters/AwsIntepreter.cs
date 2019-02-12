using System;
using System.Collections.Generic;
using Cadl.Core.Arctifact;

namespace Cadl.Core.Interpreters
{
    public class AwsIntepreter : Interpreter
    {
        public AwsIntepreter(Factory factory, Dictionary<string, object> config)
            : base(factory, config)
        {
        } 

        public override void Interpret()
        {
            throw new NotImplementedException();
        }
    }
}
