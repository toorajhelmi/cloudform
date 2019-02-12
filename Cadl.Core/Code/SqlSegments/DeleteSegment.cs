using System;
using Cadl.Core.Components;
using Cadl.Core.Interpreters;

namespace Cadl.Core.Code.SqlSegments
{
    public class DeleteSegment : CodeSegment
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
        public DeleteSegment(string methodName, Sql sql, string statement)
        {
            Requires.Add("var Request = require(\"tedious\").Request;");
            Dependencies.Add(new ConnectionSegement(sql));
            Methods.Add(updateMethod
                .Replace("#method-name", methodName)
                .Replace("#sql", statement)
                .Replace("#database", sql.DbName));
            FunctionCode = $"await {methodName}();";
        }
    }
}
