using System;
using System.Diagnostics;
using System.Linq;
using Cadl.Core.Arctifact;
using Cadl.Core.Settings;
using Cadl.Core.Components;
using System.Collections.Generic;

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
    }
}
