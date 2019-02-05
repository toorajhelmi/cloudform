using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Cadl.Core.Deployers;
using Cadl.Core.Components;

namespace Cadl.Core.Interpreters.Azure
{
    public class AzureResourceGroupInterpreter : ComponentInterpreter
    {
        public override void Interpret(string outputPath, Component component, Dictionary<string, string> config)
        {
            var assembly = this.GetType().GetTypeInfo().Assembly;
            var uniqueId = Guid.NewGuid().ToString().ToLower().Replace("-", "");
            using (var resource = assembly.GetManifestResourceStream("Cadl.Core.TF.Azure.resource_group.txt"))
            {
                using (var sr = new StreamReader(resource))
                {
                    var tfTemplate = sr.ReadToEnd();
                    var tf = tfTemplate.Replace("#region", config["region"])
                                       .Replace("#name", config["resource_group"]);

                    File.WriteAllText($"{outputPath}/{component.ComponentName}.tf", tf);
                }
            }

            //var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\\tfTemplates\Azure\Function.txt");
            //var tfTemplate = File.ReadAllText(path);
        }
    }
}
