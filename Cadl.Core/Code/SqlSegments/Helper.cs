using System;
using System.Collections.Generic;
using System.Text;

namespace Cadl.Core.Code.SqlSegments
{
    public static class Helper
    {
        public static string CreateParameters(List<Parameter> parameters)
        {
            var addParams = new StringBuilder();
            foreach (var parameter in parameters)
            {
                addParams.Append($".addParameter('{parameter.Name}', {parameter.Type}, {parameter.Name.Replace("@", "")})");
                addParams.AppendLine();
            }

            return addParams.ToString();
        }
    }
}
