using System;
using System.Collections.Generic;
using Cloudform.Core.Components;
using Cloudform.Core.Interpreters.Messages;

namespace Cloudform.Core.Arctifact
{
    public class Factory
    {
        public Factory()
        {
            OutputPath = "/Users/Tooraj/Documents/CADL/output";
        }

        public string Name { get; set; }
        public string Script { get; set; }
        public int BuildId { get; set; }
        public List<Component> Components { get; set; } = new List<Component>();
        public List<Message> Messages { get; set; } = new List<Message>();
        public string OutputPath { get; set; }
        public string TfPath => $"{OutputPath}/tf";
        public string CodePath => $"{OutputPath}/code";
        public string PackagePath => $"{OutputPath}/package";
        public Dictionary<string, object> Props { get; set; }
    }
}
