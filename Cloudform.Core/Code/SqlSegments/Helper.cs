using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Cloudform.Core.Code.SqlSegments
{
    public static class Helper
    {
        public static string CreateParameters(List<Parameter> parameters)
        {
            var addParams = new StringBuilder();

            foreach (var parameter in parameters)
            {
                addParams.Append($"request.addParameter('{parameter.Name}', {parameter.Type}, {parameter.Name.Replace("@", "")});");
                addParams.AppendLine();
            }

            return addParams.ToString();
        }
    }
}
