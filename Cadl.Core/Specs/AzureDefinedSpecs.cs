using System;
using System.Collections.Generic;

namespace Cadl.Core.Specs
{
    public class AzureDefinedSpecs : DefinedSpecs
    {
        private AzureDefinedSpecs()
        {
            Values["Function.H"].Add("size", "P1V2");
            Values["Function.M"].Add("size", "S1");
            Values["Function.S"].Add("size", "D1");
            Values["Function.F"].Add("size", "F1");
            Values["Function.C"].Add("size", "Y1");
        }

        public static AzureDefinedSpecs Instance => new AzureDefinedSpecs();
    }
}
