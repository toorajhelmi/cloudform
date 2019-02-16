using System;
using System.Collections.Generic;
using Cadl.Core.Components;
using Cadl.Core.Interpreters.Messages;

namespace Cadl.Core.Arctifact
{
    public class Factory
    {
        public Factory()
        {
            Props = new Dictionary<string, object> {
                    { "resource_group", "rg3245" },
                    { "region", "westus2" } };
            OutputPath = "/Users/Tooraj/Documents/CADL/output"; 
        }

        public string Name { get; set; }
        public string Script { get; set; }
        public List<Component> Components { get; set; } = new List<Component>();
        public List<Message> Messages { get; set; } = new List<Message>();
        public string OutputPath { get; set; } 
        public string TfPath => $"{OutputPath}/tf";
        public string CodePath => $"{OutputPath}/code";
        public string PackagePath => $"{OutputPath}/package";
        public Dictionary<string, object> Props { get; set; }
    }
}
