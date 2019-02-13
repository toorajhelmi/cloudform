using System;
using Cadl.Core.Components;
using Cadl.Core.Interpreters;

namespace Cadl.Core.Code.SqlSegments
{
    public class DeleteSegment : Segment
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
        public DeleteSegment(int indentCount, string methodName, Sql sql, string statement)
            : base(indentCount)
        {
            Requires.Add("var Request = require(\"tedious\").Request;");
            Dependencies.Add(new ConnectionSegement(indentCount, sql));
            Methods.Add(updateMethod
                .Replace("#method-name", methodName)
                .Replace("#sql", statement)
                .Replace("#database", sql.DbName));
            FunctionCode = $"await {methodName}();";
        }
    }
}
