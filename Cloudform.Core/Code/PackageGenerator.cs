using System;
using System.Collections.Generic;

namespace Cloudform.Core.Code
{
    public class PackageGenerator
    {
        private const string package =
@"
{
    ""name"": ""#name"",
    ""version"": ""1.0.0"",
    ""main"": ""index.js"",
    ""author"": ""Cloudezie Inc. (www.cloudezine.com)"",
    ""license"": ""ISC"",
    ""dependencies"" : 
    {
        #dependencies
    }
}";

        public PackageGenerator(string name, List<string> dependencies)
        {
            Package = package.Replace("#name", name)
                .Replace("#dependencies", string.Join(",\r\n", dependencies));
        }

        public string Package { get; set; }
    }
}
