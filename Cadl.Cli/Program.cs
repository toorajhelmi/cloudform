using System;
using System.IO;
using System.Reflection;
using Cadl.Core.Arctifact;

namespace Cadl.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Path.Combine($"{Environment.CurrentDirectory}/Samples/OrderScript.cadl");

            var factory = new Factory
            {
                Script = File.ReadAllText(path)
            };

            Core.Builder.Build(factory);
            Console.ReadLine();
        }
    }
}
