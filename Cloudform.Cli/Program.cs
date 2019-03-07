using System;
using System.Collections.Generic;
using System.IO;
using Cloudform.Core.Arctifact;

namespace Cloudform.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Path.Combine($"{Environment.CurrentDirectory}/Samples/OrderScript.cadl");

            var factory = new Factory
            {
                Script = File.ReadAllText(path),
                Props = new Dictionary<string, object> {
                    { "client_secret", "){BQ6{h>?-a568OG#))Y-n5V!|[b(^&" },
                    { "subscription_id", "9a4fe1a5-274e-4c67-8321-8a55ec1ea64d" },
                    { "client_id", "7ffb12bc-357e-46e5-83e2-7231372561a4" },
                    { "tenant_id", "bafa704d-560b-4ee8-9563-c265cae5ffe6" }
                }
            };

            Core.Builder.Build(factory, new EventLogger());
            Console.ReadLine();
        }
    }
}
