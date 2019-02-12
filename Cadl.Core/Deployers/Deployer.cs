using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Cadl.Core.Arctifact;
using Cadl.Core.Components;

namespace Cadl.Core.Deployers
{
    public class Deployer
    {
        private Dictionary<string, object> config;
        private Factory factory;
        private bool debug;

        public Deployer(Factory factory, Dictionary<string, object> config, bool debug = false)
        {
            this.debug = debug;
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

            var initProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = factory.TfPath,
                    FileName = "terraform",
                    RedirectStandardOutput = !debug,
                    RedirectStandardError = !debug
                }
            };
            initProcess.StartInfo.Arguments = "init";
            initProcess.Start();

            if (!debug)
            {
                while (!initProcess.StandardError.EndOfStream)
                {
                    string line = initProcess.StandardError.ReadLine();
                    if (line.IndexOf("Error") != -1)
                    {
                        canContinue = false;
                        initProcess.Close();
                        break;
                    }
                }
            }

            if (debug || canContinue)
            {
                initProcess.WaitForExit();
            }

            if (canContinue)
            {
                var applyProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        WorkingDirectory = factory.TfPath,
                        FileName = "terraform",
                        RedirectStandardOutput = !debug,
                        RedirectStandardError = !debug
                    }
                };
                applyProcess.StartInfo.Arguments = "apply -auto-approve";
                applyProcess.Start();

                if (!debug)
                {
                    while (!applyProcess.StandardError.EndOfStream)
                    {
                        string line = applyProcess.StandardError.ReadLine();
                        if (line.IndexOf("Error") != -1)
                        {
                            canContinue = false;
                            applyProcess.Close();
                            break;
                        }
                    }
                }

                if (debug || canContinue)
                {
                    applyProcess.WaitForExit();
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
    }
}
