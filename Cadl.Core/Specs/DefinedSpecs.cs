using System;
using System.Collections.Generic;

namespace Cadl.Core.Specs
{
    public class DefinedSpecs
    {
        public Dictionary<string, Dictionary<string, string>> Values { get; set; }
            = new Dictionary<string, Dictionary<string, string>>();

        public DefinedSpecs()
        {
            Values.Add("Function.H", new Dictionary<string, string>());
            Values.Add("Function.M", new Dictionary<string, string>());
            Values.Add("Function.S", new Dictionary<string, string>());
            Values.Add("Function.F", new Dictionary<string, string>());
            Values.Add("Function.C", new Dictionary<string, string>()); //Consumption
        }
    }
}
