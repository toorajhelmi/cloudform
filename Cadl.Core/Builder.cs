using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Cadl.Core.Deployers;
using Cadl.Core.Interpreters;
using Cadl.Core.Extensions;
using Cadl.Core.Arctifact;
using Cadl.Core.Settings;

namespace Cadl.Core
{
    public class Builder
    {
        public static void Build(Factory factory)
        {
            var cloudConfiguration = new CloudConfiguration();
            Console.WriteLine("> Parsing Script");
            var parser = new Cadl.Core.Parsers.Parser();
            parser.Parse(factory);

            Interpreter interpreter = null;
            var cloud = TargetCloud.Azure;

            var azureConfig = DictionaryEx.Combine(
                cloudConfiguration.GlobalConfig,
                cloudConfiguration.AzureConfig,
                factory.Props);

            switch (cloud)
            {
                case TargetCloud.Azure: interpreter = new AzureInterpreter(factory, azureConfig); break;
                case TargetCloud.Aws: interpreter = new AwsIntepreter(factory, null); break;
                case TargetCloud.Gcp: interpreter = new GcpInterpreter(factory, null); break;
            }

            if (Directory.Exists(factory.OutputPath))
            {
                Directory.Delete(factory.OutputPath, true);
            }

            Directory.CreateDirectory(factory.OutputPath);

            interpreter.Interpret();

            Deployer deployer = null;
            switch (cloud)
            {
                case TargetCloud.Azure: deployer = new AzureDeployer(factory, azureConfig, true); break;
                default: deployer = new Deployer(factory, azureConfig, true); break;
            }

            deployer.Deploy();
        }
    }
}
