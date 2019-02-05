using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Cadl.Core.Components;

namespace Cadl.Core.Interpreters.Azure
{
    public class AzureFunctionInterpreter : ComponentInterpreter
    {
        public override void Interpret(string outputPath, Component component, Dictionary<string, string> config)
        {
            var function = component as Function;
            var assembly = this.GetType().GetTypeInfo().Assembly;
            var uniqueId = Guid.NewGuid().ToString().ToLower().Replace("-", "");
            using (var resource = assembly.GetManifestResourceStream("Cadl.Core.TF.Azure.function.txt"))
            {
                using (var sr = new StreamReader(resource))
                {
                    var tfTemplate = sr.ReadToEnd();
                    var tf = tfTemplate.Replace("#resource_group", config["resource_group"])
                                       .Replace("#region", config["region"])
                                       .Replace("#fn_sp_name", config["fn_sp_name"])
                                       .Replace("#fn_sa_name", config["fn_sa_name"])
                                       .Replace("#uniqueId", uniqueId)
                                       .Replace("#name", function.FunctionName);
                                        
                    File.WriteAllText($"{outputPath}/{component.ComponentName}.tf", tf);
                }
            }

            //var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\\tfTemplates\Azure\Function.txt");
            //var tfTemplate = File.ReadAllText(path);

                             
        }
    }
}
