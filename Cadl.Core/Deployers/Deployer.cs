using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Cadl.Core.Arctifact;
using Cadl.Core.Components;
using Cadl.Core.Settings;
using Cadl.Core.Extensions;

namespace Cadl.Core.Deployers
{
    public class Deployer
    {
        private const string terraform = "terraform";
        protected Dictionary<string, object> config;
        protected Factory factory;

        public Deployer(Factory factory, Dictionary<string, object> config)
        {
            this.config = config;
            this.factory = factory;
        }

        public void Deploy()
        {
            GenerateCredFile();
            var canContinue = RunTf();

            if (canContinue)
            {
                Console.WriteLine("Starting initialization...");

                foreach (var sql in factory.Components.OfType<Sql>())
                {
                    SqlDeployer.PopulateSql(sql, config);
                }
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

        private bool RunTf()
        {
            var canContinue = true;

            var initProcess = ProcessEx.Create(factory.TfPath, terraform, "init");
            canContinue = initProcess.StopAtError();

            if (ApplicationSettings.Debug || canContinue)
            {
                initProcess.WaitForExit();
            }

            if (canContinue)
            {
                var applyProcess = ProcessEx.Create(factory.TfPath, terraform, "apply -auto-approve");
                canContinue = applyProcess.StopAtError();

                if (ApplicationSettings.Debug || canContinue)
                {
                    applyProcess.WaitForExit();
                }

                if (canContinue)
                {
                    canContinue = DeployCloudSpecific();
                }

                if (canContinue)
                {
                    Console.WriteLine("Deployed all components.");
                }
                else
                {
                    Console.WriteLine("Resource creation failed.");
                }
            }

            return canContinue;
        }

        protected virtual bool DeployCloudSpecific()
        {
            return true;
        }
    }
}
