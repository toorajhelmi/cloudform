using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Cloudform.Core.Arctifact;
using Cloudform.Core.Code;
using Cloudform.Core.Components;
using Cloudform.Core.Extensions;

namespace Cloudform.Core.Interpreters
{
    public class AzureInterpreter : Interpreter
    {
        private const int maxNameLength = 24;

        public AzureInterpreter(Factory factory)
            : base(factory)
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
                var storageAccountName = NameGenerator.Unique("fnsa", maxNameLength);
                props["fn_sa_name"] = storageAccountName;
                props["name"] = storageAccountName;
                GenerateTf("storage_account", storageAccountName.ToLower());

                var sizes = factory.Components.OfType<Function>().GroupBy(c => c.Size);
                foreach (var size in sizes)
                {
                    var spName = NameGenerator.Unique($"fnsp{size.Key}", maxNameLength);
                    props["fn_sp_name"] = spName.ToLower();
                    props["name"] = spName;
                    props["size"] = size.Key;
                    GenerateTf("service_plan", spName);

                    foreach (var function in size.ToList())
                    {
                        var cadlInterpreter = new CadlInterpreter();

                        props["name"] = function.FunctionName;
                        if (function.OutputQueue != null)
                        {
                            props["q_sa_name"] = function.OutputQueue.StorageAccount;
                        }

                        GenerateTf("function", function.ComponentName);

                        var functionFolder = $"{factory.CodePath}/{function.FunctionName}/{Function.FolderName}";
                        Directory.CreateDirectory($"{factory.CodePath}/{function.FunctionName}");
                        Directory.CreateDirectory(functionFolder);

                        var js = cadlInterpreter.CompileToJs(function, factory.Components, props);
                        File.WriteAllText($"{functionFolder}/index.js", js);

                        var packageGeneratpr = new PackageGenerator(function.FunctionName, cadlInterpreter.DependsOnModules);
                        File.WriteAllText($"{factory.CodePath}/{function.FunctionName}/package.json", packageGeneratpr.Package);
                    }
                }
            }
        }

        private void BuildSql()
        {
            if (factory.Components.Any(c => c is Sql))
            {
                var serverName = NameGenerator.Unique("dbs", maxNameLength);
                props["name"] = serverName;
                props["server_name"] = serverName;
                GenerateTf("sql_server");

                //Detect external IP
                var externalip = new WebClient().DownloadString("http://checkip.amazonaws.com/").Trim(new[] { '\n' });
                props["ip_addr"] = externalip;
                props["name"] = $"serverName-{"fw"}";
                GenerateTf("sql_firewall_rule");

                foreach (var sql in factory.Components.OfType<Sql>())
                {
                    sql.ServerName = serverName;
                    props["name"] = sql.DbName;
                    props["dbname"] = sql.DbName;
                    GenerateTf("sql_db", sql.DbName);
                }
            }
        }

        private void BuildQueues()
        {
            if (factory.Components.Any(c => c is Queue))
            {
                //Create storage account to hold the queues
                var storageAccountName = NameGenerator.Unique($"qsa", maxNameLength);
                props["q_sa_name"] = storageAccountName.ToLower();
                props["name"] = storageAccountName;
                GenerateTf("storage_account", storageAccountName);

                foreach (var queue in factory.Components.OfType<Queue>())
                {
                    queue.StorageAccount = storageAccountName;
                    props["name"] = queue.QueueName;
                    GenerateTf("storage_queue", queue.ComponentName);
                }
            }
        }

        private void GenerateTf(string templateName, string outputName = null)
        {
            props["tf_name"] = props["name"].ToString().ToLower();

            var assembly = this.GetType().GetTypeInfo().Assembly;
            using (var resource = assembly.GetManifestResourceStream($"Cloudform.Core.TF.Azure.{templateName}.txt"))
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
