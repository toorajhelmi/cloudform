using System;
using System.Diagnostics;
using Cadl.Core.Settings;

namespace Cadl.Core.Extensions
{
    public static class ProcessEx
    {
        public static bool StopAtError(this Process process)
        {
            var succeeded = true;
            var debug = ApplicationSettings.Debug;

            process.StartInfo.RedirectStandardOutput = !debug;
            process.StartInfo.RedirectStandardError = !debug;

            process.Start();

            if (!debug)
            {
                while (!process.StandardError.EndOfStream)
                {
                    string line = process.StandardError.ReadLine();
                    if (line.IndexOf("Error", StringComparison.InvariantCultureIgnoreCase) != -1)
                    {
                        succeeded = false;
                        process.Close();
                        break;
                    }
                }
            }

            return succeeded;
        }

        public static Process Create(string directory, string command, string args)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = directory,
                    FileName = command,
                }
            };
            process.StartInfo.Arguments = args;

            return process;
        }
    }
}
