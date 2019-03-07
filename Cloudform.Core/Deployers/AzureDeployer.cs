using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Cloudform.Core.Arctifact;
using Cloudform.Core.Code.Azure;
using Cloudform.Core.Components;
using Cloudform.Core.Extensions;
using Cloudform.Core.Reporting;
using Cloudform.Core.Settings;

namespace Cloudform.Core.Deployers
{
    public class AzureDeployer : Deployer
    {
        private const string az = "az";
        public AzureDeployer(Factory factory, IEventLogger eventLogger, bool debug = false)
            : base(factory, eventLogger)
        {
        }

        protected override void DeployCloudSpecific()
        {
            SetQueueConnectionString();

            foreach (var function in factory.Components.OfType<Function>())
            {
                AddFunctionBinding(function);
                AddHostFile($"{factory.CodePath}/{function.FunctionName}");
            }

            DeployCode();
        }

        private void SetQueueConnectionString()
        {
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
                        RedirectStandardError = !ApplicationSettings.Debug
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

                if (!ApplicationSettings.Debug)
                {
                    while (!outputProcess.StandardError.EndOfStream)
                    {
                        string line = outputProcess.StandardError.ReadLine();
                        if (line.IndexOf("Error", StringComparison.InvariantCultureIgnoreCase) != -1)
                        {
                            outputProcess.Close();
                            throw new DeployingException(DeployingException.QueueConnectionFailed, queue.QueueName);
                        }
                    }
                }
            }
        }

        private void AddFunctionBinding(Function function)
        {
            File.WriteAllText($"{factory.CodePath}/{function.FunctionName}/{Function.FolderName}/function.json",
                new BindingGenerator(function).Bindings);
        }

        private void AddHostFile(string desinationPath)
        {
            File.WriteAllText($"{desinationPath}/host.json",
                @"{
                    ""version"": ""2.0""
                }");
        }

        private void InstallModules(string functionFolder)
        {
            var installProcess = ProcessEx.Create(functionFolder, "npm", "install");
            if (!installProcess.StopAtError())
            {
                throw new DeployingException(DeployingException.ModuleInstallationFailed);
            }
        }

        private void DeployCode()
        {
            //az login --service-principal -u "http://my-app" -p <password> --tenant <tenant>
            var loginProcess = ProcessEx.Create(factory.CodePath, az,
                $"login --service-principal -u {config["client_id"]} -p {config["client_secret"]} --tenant {config["tenant_id"]}");
            if (!loginProcess.StopAtError())
            {
                throw new DeployingException(DeployingException.AzureLoginFailed);
            }

            Directory.CreateDirectory(factory.PackagePath);

            foreach (var function in factory.Components.OfType<Function>())
            {
                var functionFolder = $"{factory.CodePath}/{function.FunctionName}";
                InstallModules(functionFolder);

                //az functionapp deployment source config-zip -g test -n test1543  --src /Users/Tooraj/MyFunctionProj/HttpTrigger.zip
                var packagePath = $"{factory.PackagePath}/{function.FunctionName}.zip";
                ZipFile.CreateFromDirectory($"{factory.CodePath}/{function.FunctionName}", packagePath);
                var rg = config["resource_group"];
                var pushCodeProcess = ProcessEx.Create(factory.CodePath, az,
                    $"functionapp deployment source config-zip -g {rg} -n {function.FunctionName}  --src {packagePath}");
                if (!pushCodeProcess.StopAtError())
                {
                    throw new DeployingException(DeployingException.CodePushFailed);
                }
            }
        }
    }
}
