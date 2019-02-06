using System;
using System.Collections.Generic;
using System.IO;
using Cadl.Core.Deployers;
using Cadl.Core.Interpreters;
using Cadl.Core.Interpreters.Aws;
using Cadl.Core.Interpreters.Azure;
using Cadl.Core.Interpreters.Gcp;

namespace Cadl.Core
{
    public class Builder
    {
        public Builder()
        {
        }

        public static void Build(string script)
        {
            Console.WriteLine("> Parsing Script");
            var parser = new Cadl.Core.Parsers.Parser();
            parser.Parse(script);


            Interpreter interpreter = null;
            var cloud = TargetCloud.Azure;

            var azureConfig = new Dictionary<string, string> { 
                { "resource_group", "rg3245" },
                { "region", "westus2" },
            };
            switch (cloud)
            {
                case TargetCloud.Azure: interpreter = new AzureInterpreter(azureConfig); break;
                case TargetCloud.Aws:interpreter = new AwsIntepreter(null); break;
                case TargetCloud.Gcp:interpreter = new GcpInterpreter(null); break;
            }

            var outputPath = "/Users/Tooraj/Documents/CADL/output";

            Directory.Delete(outputPath, true);
            Directory.CreateDirectory(outputPath);

            interpreter.Interpret(outputPath, parser.Components, parser.Messages);

            var azureCreds = new Dictionary<string, string> {
                { "client_secret", "){BQ6{h>?-a568OG#))Y-n5V!|[b(^&" },
                { "subscription_id", "9a4fe1a5-274e-4c67-8321-8a55ec1ea64d" },
                { "client_id", "7ffb12bc-357e-46e5-83e2-7231372561a4" },
                { "tenant_id", "bafa704d-560b-4ee8-9563-c265cae5ffe6" },
                { "sql_admin", "admin2020"},
                { "sql_password", "password1!"},
                { "azure_user", "tooraj@me.com" },
                { "azure_pass", "Hapalu2015!" },

            };
            var deployer = new Deployer();
            deployer.Deploy(outputPath, azureCreds, parser.Components);
        }
    }
}
