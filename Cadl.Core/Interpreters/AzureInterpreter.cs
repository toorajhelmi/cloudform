using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Cadl.Core.Arctifact;
using Cadl.Core.Components;

namespace Cadl.Core.Interpreters
{
    public class AzureInterpreter : Interpreter
    {
        public AzureInterpreter(Factory factory, Dictionary<string, object> config)
            : base(factory, config)
        {
        }

        public override void Interpret()
        {
            BuildProject();
            BuildQueues();
            BuildSql();
            BuildFunctions();
        }

        private void BuildProject()
        {
            Directory.CreateDirectory(factory.TfPath);
            Directory.CreateDirectory(factory.CodePath);

            props["name"] = "provider";
            GenerateTf("provider");

            props["name"] = "variables";
            GenerateTf("variables");

            props["name"] = props["resource_group"];
            GenerateTf("resource_group");
        }

        private void BuildFunctions()
        {
            if (factory.Components.Any(c => c is Function))
            {
                var storageAccountName = $"fnsa{props["resource_group"]}";
                props["fn_sa_name"] = storageAccountName;
                props["name"] = $"fnsa{props["resource_group"]}";
                GenerateTf("storage_account", storageAccountName.ToLower());

                var sizes = factory.Components.OfType<Function>().GroupBy(c => c.Size);
                foreach (var size in sizes)
                {
                    var spName = $"fnsp{size.Key}{props["resource_group"]}";
                    props["fn_sp_name"] = spName.ToLower();
                    props["name"] = spName;
                    props["size"] = size.Key;
                    GenerateTf("service_plan", spName);

                    foreach (var function in size.ToList())
                    {
                        var cadlInterpreter = new CadlInterpreter();

                        props["name"] = function.FunctionName;
                        GenerateTf("function", function.ComponentName);

                        var js = cadlInterpreter.CompileToJs(function, props);
                        Directory.CreateDirectory($"{factory.CodePath}/{function.FunctionName}");
                        File.WriteAllText($"{factory.CodePath}/{function.FunctionName}/index.js", js);
                    }
                }
            }
        }

        private void BuildSql()
        {
            if (factory.Components.Any(c => c is Sql))
            {
                var serverName = $"dbserver{props["resource_group"]}";
                props["name"] = serverName;
                props["server_name"] = serverName;
                GenerateTf("sql_server");

                //Detect external IP
                var externalip = new WebClient().DownloadString("http://icanhazip.com").Trim(new[] { '\n' });
                props["ip_addr"] = externalip;
                props["name"] = $"serverName-{"fw"}";
                GenerateTf("sql_firewall_rule");

                foreach (var sql in factory.Components.OfType<Sql>())
                {
                    sql.ServerName = serverName;
                    props["name"] = sql.DbName;
                    props["dbname"] = sql.DbName;
                    props[$"db-{sql.DbName}"] = new DbInfo
                    {
                        Database = sql.DbName,
                        Password = props["sql_password"].ToString(),
                        Server = $"{sql.ServerName}.database.windows.net",
                        Username = props["sql_admin"].ToString(),
                    };
                    GenerateTf("sql_db", sql.DbName);
                }
            }
        }

        private void BuildQueues()
        {
            if (factory.Components.Any(c => c is Queue))
            {
                //Create storage account to hold the queues
                var storageAccountName = $"qsa{props["resource_group"]}";
                props["q_sa_name"] = storageAccountName.ToLower();
                props["name"] = storageAccountName;
                GenerateTf("storage_account", storageAccountName);

                foreach (var queue in factory.Components.OfType<Queue>())
                {
                    props["name"] = queue.QueueName;
                    GenerateTf("storage_queue", queue.ComponentName);
                }
            }
        }

        private void GenerateTf(string templateName, string outputName = null)
        {
            props["tf_name"] = props["name"].ToString().ToLower();

            var assembly = this.GetType().GetTypeInfo().Assembly;
            using (var resource = assembly.GetManifestResourceStream($"Cadl.Core.TF.Azure.{templateName}.txt"))
            {
                using (var sr = new StreamReader(resource))
                {
                    var tfTemplate = sr.ReadToEnd();

                    foreach (var prop in props.Keys)
                    {
                        tfTemplate = tfTemplate.Replace($"#{prop}", props[prop].ToString());
                    }

                    File.WriteAllText($"{factory.TfPath}/{outputName ?? templateName}.tf", tfTemplate);
                }
            }
        }
    }
}
