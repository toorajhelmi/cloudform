using System;
using System.Diagnostics;
using System.Linq;
using Cadl.Core.Arctifact;
using Cadl.Core.Settings;
using Cadl.Core.Components;
using System.Collections.Generic;
using Cadl.Core.Code.Azure;
using System.IO;

namespace Cadl.Core.Deployers
{
    public class AzureDeployer : Deployer
    {
        public AzureDeployer(Factory factory, Dictionary<string, object> config, bool debug = false)
            : base(factory, config, debug)
        {
        }

        protected override bool DeployCloudSpecific()
        {
            var canContinue = SetQueueConnectionString();

            if (!canContinue)
            {
                return false;
            }

            foreach (var function in factory.Components.OfType<Function>())
            {
                AddFunctionBinding(function);
            }

            AddHostFile();

            return true;
        }

        private bool SetQueueConnectionString()
        {
            var canContinue = true;

            //Retrieve properites like connection string
            foreach (var queue in factory.Components.OfType<Queue>())
            {
                var outputProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        WorkingDirectory = factory.TfPath,
                        FileName = $"terraform",
                        RedirectStandardOutput = true,
                        RedirectStandardError = !debug
                    }
                };
                outputProcess.StartInfo.Arguments = $"output {queue.StorageAccount}_connection_string";
                outputProcess.Start();

                while (!outputProcess.StandardOutput.EndOfStream)
                {
                    string line = outputProcess.StandardOutput.ReadLine();
                    if (line.IndexOf("DefaultEndpointsProtocol") != -1)
                    {
                        queue.ConnectionString = line.Trim();
                        outputProcess.Close();
                        break;
                    }
                }

                if (!debug)
                {
                    while (!outputProcess.StandardError.EndOfStream)
                    {
                        string line = outputProcess.StandardError.ReadLine();
                        if (line.IndexOf("Error") != -1)
                        {
                            canContinue = false;
                            outputProcess.Close();
                            break;
                        }
                    }
                }

                if (!canContinue)
                {
                    break;
                }
            }

            return canContinue;
        }

        private void AddFunctionBinding(Function function)
        {
            File.WriteAllText($"{factory.CodePath}/{function.FunctionName}/function.json",
                new BindingGenerator(function).Bindings);
        }

        private void AddHostFile()
        {
            File.WriteAllText($"{factory.CodePath}/host.json",
                @"{
                    ""version"": ""2.0""
                }");
        }
    }
}
