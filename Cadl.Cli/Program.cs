using System;
using System.IO;
using System.Reflection;

namespace Cadl.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Path.Combine($"{Environment.CurrentDirectory}/Samples/OrderScript.cadl");
            var script = File.ReadAllText(path);
            Core.Builder.Build(script);
            Console.ReadLine();
        }
    }
}
