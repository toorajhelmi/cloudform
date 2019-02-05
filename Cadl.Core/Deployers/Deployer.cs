using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Cadl.Core.Components;
using System.Data.SqlClient;

namespace Cadl.Core.Deployers
{
    public class Deployer
    {
        private string path;
        private Dictionary<string, string> credentials;

        public void Deploy(string path, Dictionary<string, string> credentials, List<Component> components)
        {
            this.credentials = credentials;
            this.path = path;
            GenerateCredFile();
            RunTf();

            foreach(var sql in components.OfType<Sql>())
            {
                SqlDeployer.PopulateSql(sql, credentials);
            }
        }

        private void GenerateCredFile()
        {
            using (var sw = File.CreateText($"{path}/terraform.tfvars"))
            {
                foreach (var cred in credentials)
                {
                    sw.WriteLine($"{cred.Key}=\"{cred.Value}\"");
                }
            }
        }

        private void RunTf()
        {
            var initProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = path,
                    FileName = "terraform"
                }
            };
            initProcess.StartInfo.Arguments = "init";
            initProcess.Start();
            initProcess.WaitForExit();

            var applyProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = path,
                    FileName = "terraform"
                }
            };
            applyProcess.StartInfo.Arguments = "apply -auto-approve";
            applyProcess.Start();
        }
    }
}
