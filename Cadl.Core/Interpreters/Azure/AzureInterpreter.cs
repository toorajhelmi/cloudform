using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Cadl.Core.Components;
using Cadl.Core.Interpreters.Messages;
using System.Net;

namespace Cadl.Core.Interpreters.Azure
{
    public class AzureInterpreter : Interpreter
    {
        private string outputPath;
        private Dictionary<string, string> props;

        public AzureInterpreter(Dictionary<string, string> config)
            : base(config)
        {
        }

        public override void Interpret(string outputPath, List<Components.Component> components, List<Message> messages)
        {
            this.outputPath = outputPath;
            props = config;

            props["name"] = "provider";
            GenerateTf("provider");

            props["name"] = "variables";
            GenerateTf("variables");

            props["name"] = config["resource_group"];
            GenerateTf("resource_group");

            GenerateSqlTf(components);
            GenerateFunctionTf(components);
        }

        private void GenerateFunctionTf(List<Component> components)
        {
            if (components.Any(c => c is Function))
            {
                props["fn_sa_name"] = $"fnsa{config["resource_group"]}";
                props["name"] = $"fnsa{config["resource_group"]}";
                GenerateTf("storage_account");

                var sizes = components.OfType<Function>().GroupBy(c => c.Size);
                foreach (var size in sizes)
                {
                    var spName = $"fnsp{size.Key}{config["resource_group"]}";
                    props["fn_sp_name"] = spName;
                    props["name"] = spName;
                    props["size"] = size.Key;
                    GenerateTf("service_plan", spName);

                    foreach (var function in size.ToList())
                    {
                        props["name"] = function.ComponentName;
                        GenerateTf("function");
                    }
                }
            }
        }

        private void GenerateSqlTf(List<Component> components)
        {
            if (components.Any(c => c is Sql))
            {
                var serverName = $"dbserver{config["resource_group"]}";
                props["name"] = serverName;
                props["server_name"] = serverName;
                GenerateTf("sql-server");

                //Detect external IP
                var externalip = new WebClient().DownloadString("http://icanhazip.com").Trim(new []{'\n'});
                props["ip_addr"] = externalip;
                props["name"] = $"serverName-{"fw"}";
                GenerateTf("sql-firewall-rule");

                foreach (var sql in components.OfType<Sql>())
                {
                    sql.ServerName = serverName;
                    props["name"] = sql.ComponentName;
                    props["dbname"] = sql.DbName;
                    GenerateTf("sql-db");
                }
            }
        }

        private void GenerateTf(string templateName, string outputName = null)
        {
            var assembly = this.GetType().GetTypeInfo().Assembly;
            using (var resource = assembly.GetManifestResourceStream($"Cadl.Core.TF.Azure.{templateName}.txt"))
            {
                using (var sr = new StreamReader(resource))
                {
                    var tfTemplate = sr.ReadToEnd();

                    foreach (var prop in props.Keys)
                    {
                        tfTemplate = tfTemplate.Replace($"#{prop}", props[prop]);
                    }

                    File.WriteAllText($"{outputPath}/{outputName ?? templateName}.tf", tfTemplate);
                }
            }
        }
    }
}
