using System;
using Cadl.Core.Interpreters;

namespace Cadl.Core.Code.Sql
{
    public class UpdateSegment : CodeSegment
    {
        private string updateMethod = @"
function #method-name()
{
    return new Promise(function (resolve, reject) {
        connectTo#database();
        #database_connection.on('connect', function (err) {
            if (err) {
                console.log(err);
                reject(err);
            } else {
                console.log('Running ' + '#method-name');
                var query = #sql;

                var request = new Request(query, function (
                    err,
                    rowCount,
                    rows) {
                        resolve();
                    });

                #database_connection.execSql(request);
            }
        });    
    });
}";
        public UpdateSegment(string methodName, string sql, DbInfo dbInfo)
        {
            Requires.Add("var Request = require(\"tedious\").Request;");
            Dependencies.Add(new ConnectionSegement(dbInfo));
            Methods.Add(updateMethod
                .Replace("#method-name", methodName)
                .Replace("#sql", sql)
                .Replace("#database", dbInfo.Database));
            FunctionCode = $"await {methodName}();";
        }
    }
}
