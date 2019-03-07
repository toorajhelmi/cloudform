using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Cloudform.Core.Deployers;
using Cloudform.Core.Interpreters;
using Cloudform.Core.Extensions;
using Cloudform.Core.Arctifact;
using Cloudform.Core.Settings;
using System.Threading.Tasks;
using Cloudform.Core.Reporting;

namespace Cloudform.Core
{
    public class Builder
    {
        public static async Task Build(Factory factory, IEventLogger eventLogger)
        {
            await Task.Run(() =>
            {
                var cloudConfiguration = new CloudConfiguration();
                eventLogger.Log(factory.BuildId, "Parsing CADL Script");
                var parser = new Cloudform.Core.Parsers.Parser();
                parser.Parse(factory, eventLogger);

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
                    case TargetCloud.Azure: deployer = new AzureDeployer(factory, eventLogger, true); break;
                }

                deployer.Deploy();
            });
        }
    }
}
