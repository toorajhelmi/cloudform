using System;
using System.Collections.Generic;
using Cadl.Core.Components;
using Cadl.Core.Interpreters.Messages;

namespace Cadl.Core.Interpreters.Aws
{
    public class AwsIntepreter : Interpreter
    {
        public AwsIntepreter(Dictionary<string, string> config)
            : base(config)
        {
        } 

        public override void Interpret(string outputPath, List<Component> components, List<Message> messages)
        {
            throw new NotImplementedException();
        }
    }
}
