using System.Linq;
using System.Collections.Generic;
using Cloudform.Core.Components;

namespace Cloudform.Core.Code.SqlSegments
{
    public class UpdateSegment : Segment
    {
        private const string updateMethod = @"
async function #method-name(#parameters)
{
    return new Promise(function (resolve, reject) {
        var query = #sql;
        var request = new Request(query, function (
            err,
            rowCount,
            rows) {
                resolve();
            });
        #add-params

        if (#database_connected) {
            #database_connection.execSql(request);
        } else {
            connectTo#database();
            #database_connection.on('connect', function (err) {
                if (err) {
                    console.log(err);
                    reject(err);
                } else {
                    console.log('Running ' + '#method-name');
                    #database_connection.execSql(request);
                }
            }); 
        }
    });
}";

        public UpdateSegment(int indentCount, string methodName, Sql sql, string statement,
            List<Parameter> parameters)
            : base(indentCount)
        {
            Requires.Add("var Request = require('tedious').Request;");
            DependsOnSegments.Add(new ConnectionSegement(indentCount, sql));
            DependsOnModules.Add("\"tedious\": \"5.0.3\"");
            Methods.Add(updateMethod
                .Replace("#method-name", methodName)
                .Replace("#sql", statement)
                .Replace("#database", sql.DbName)
                .Replace("#parameters", string.Join(',', parameters.Select(p => p.Name.Replace("@", ""))))
                .Replace("#add-params", Helper.CreateParameters(parameters)));
            FunctionCode = $"await {methodName}({string.Join(',', parameters.Select(p => p.Value)).Trim()});";
        }
    }
}
