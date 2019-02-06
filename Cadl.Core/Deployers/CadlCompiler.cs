using System;
using System.Collections.Generic;
using Cadl.Core.Parsers;

namespace Cadl.Core.Deployers
{
    public class CadlCompiler
    {
        public void CompileToJs(List<Line> cadl)
        {
            foreach (var line in cadl)
            {
                if (line.KeyExists("Call"))
                {

                }
                else if (line.KeyExists("Return"))
                {

                }
                else if (line.KeyExists("Select"))
                {

                }
                else if (line.KeyExists("Insert"))
                {

                }
                else if (line.KeyExists("Update"))
                {

                }
                else if (line.KeyExists("Take"))
                {

                }
                else if (line.KeyExists("Code"))
                {

                }
                else if (line.KeyExists("Delete"))
                {

                }
            }
        }

        private string CompileCall(Line line)
        {
            return "";
        }
    }
}
