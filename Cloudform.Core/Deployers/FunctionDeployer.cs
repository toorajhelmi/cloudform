﻿using System;
using System.IO;
using Cloudform.Core.Code;
using Cloudform.Core.Components;

namespace Cloudform.Core.Deployers
{
    public class FunctionDeployer
    {
        public static void AzureDeploy(Function function, string outputPath)
        {
            var templateFolder = "";
            switch (function.Trigger)
            {
                case Trigger.Request: templateFolder = "HttpRequest"; break;
                case Trigger.Timer: templateFolder = "PeriodicCall"; break;
            }

            var codePath = $"{outputPath}/Code";
            Directory.CreateDirectory(codePath);
            CopyFile(templateFolder, "function.json", codePath);
            CopyFile("Azure", "host.json", codePath);
        }

        public static void AwsDeploy(Function function, string outputPath)
        {
            Directory.CreateDirectory($"{outputPath}/Code");
            switch (function.Trigger)
            {
                case Trigger.Request:
                    break;
                case Trigger.Timer:
                    break;
            }
        }

        private static void CopyFile(string sourcePath, string sourceName, string outputPath)
        {
            var assembly = typeof(FunctionDeployer).Assembly;
            using (var resource = assembly.GetManifestResourceStream($"Cloudform.Core.Code.{sourcePath}.{sourceName}"))
            {
                using (var sr = new StreamReader(resource))
                {
                    File.WriteAllText($"{outputPath}/{sourceName}", sr.ReadToEnd());
                }
            }
        }
    }
}
