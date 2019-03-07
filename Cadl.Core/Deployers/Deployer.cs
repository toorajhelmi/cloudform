using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cloudform.Core.Arctifact;
using Cloudform.Core.Components;
using Cloudform.Core.Extensions;
using Cloudform.Core.Settings;

namespace Cloudform.Core.Deployers
{
    public class Deployer
    {
        private const string terraform = "terraform";
        protected Dictionary<string, object> config;
        protected Factory factory;

        public Deployer(Factory factory)
        {
            this.config = factory.Props;
            this.factory = factory;
        }

        public void Deploy()
        {
            GenerateCredFile();
            RunTf();

            Console.WriteLine("Starting initialization...");

            foreach (var sql in factory.Components.OfType<Sql>())
            {
                SqlDeployer.PopulateSql(sql, config);
            }
        }

        private void GenerateCredFile()
        {
            using (var sw = File.CreateText($"{factory.TfPath}/terraform.tfvars"))
            {
                foreach (var cred in config)
                {
                    sw.WriteLine($"{cred.Key}=\"{cred.Value}\"");
                }
            }
        }

        private void RunTf()
        {
            var initProcess = ProcessEx.Create(factory.TfPath, terraform, "init");
            if (!initProcess.StopAtError())
            {
                throw new DeployingException(DeployingException.DeploymentinitializionFailed);
            }

            if (ApplicationSettings.Debug)
            {
                initProcess.WaitForExit();
            }

            var applyProcess = ProcessEx.Create(factory.TfPath, terraform, "apply -auto-approve");
            if (!applyProcess.StopAtError())
            {
                throw new DeployingException(DeployingException.DeploymentExecutionFailed);
            }

            if (ApplicationSettings.Debug)
            {
                applyProcess.WaitForExit();
            }

            DeployCloudSpecific();
            Console.WriteLine("Deployed all components.");
        }

        protected virtual void DeployCloudSpecific()
        {
        }
    }
}
