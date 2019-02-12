using System;
using Cadl.Core.Components;
using Cadl.Core.Interpreters;

namespace Cadl.Core.Code.SqlSegments
{
    public class InsertSegment : CodeSegment
    {
        private string insertMethod = @"
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
                    console.log('Received ' + rowCount);
                    if (rows) {
                        var id = rows[0][0].value;
                        console.log(id);
                        resolve(id);
                    }
                    else {
                        console.log('Insert Failed.');
                        resolve(null);
                    });

                #database_connection.execSql(request);
            }
        });    
    });
}";
        public InsertSegment(string methodName, Sql sql, string statement, string entityId)
        {
            Requires.Add("var Request = require(\"tedious\").Request;");
            Dependencies.Add(new ConnectionSegement(sql));
            Methods.Add(insertMethod
                .Replace("#method-name", methodName)
                .Replace("#sql", statement)
                .Replace("#database", sql.DbName));
            if (!string.IsNullOrWhiteSpace(entityId))
            {
                FunctionCode = $"{entityId} = await {methodName}();";
            }
            else
            {
                FunctionCode = $"await {methodName}();";
            }
        }
    }
}
