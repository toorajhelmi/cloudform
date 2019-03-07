using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Cloudform.Core.Deployers;
using Cloudform.Core.Interpreters;
using Cloudform.Core.Extensions;
using Cloudform.Core.Arctifact;
using Cloudform.Core.Settings;

namespace Cloudform.Core
{
    public class Builder
    {
        public static void Build(Factory factory)
        {
            var cloudConfiguration = new CloudConfiguration();
            Console.WriteLine("> Parsing Script");
            var parser = new Cloudform.Core.Parsers.Parser();
            parser.Parse(factory);

            Interpreter interpreter = null;
            var cloud = TargetCloud.Azure;

            //var azureConfig = DictionaryEx.Combine(
                //cloudConfiguration.GlobalConfig,
                //cloudConfiguration.AzureConfig,
                //factory.Props);


            switch (cloud)
            {
                case TargetCloud.Azure: interpreter = new AzureInterpreter(factory); break;
                case TargetCloud.Aws: interpreter = new AwsIntepreter(factory); break;
                case TargetCloud.Gcp: interpreter = new GcpInterpreter(factory); break;
            }

            if (Directory.Exists(factory.OutputPath))
            {
                Directory.Delete(factory.OutputPath, true);
            }

            var dirInfo = Directory.CreateDirectory(factory.OutputPath);

            interpreter.Interpret();

            Deployer deployer = null;
            switch (cloud)
            {
                case TargetCloud.Azure: deployer = new AzureDeployer(factory, true); break;
            }

            deployer.Deploy();
        }
    }
}
