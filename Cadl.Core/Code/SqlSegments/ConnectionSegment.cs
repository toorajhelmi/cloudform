using Cloudform.Core.Components;

namespace Cloudform.Core.Code.SqlSegments
{
    public class ConnectionSegement : Segment
    {
        private const string connect = @"
function connectTo#database() {
    if (#database_connection == null) {
        var config = {
            userName: '#username',
            password: '#password',
            server: '#connectinString',
            options: {
                database: '#database',
                encrypt: true,
                rowCollectionOnRequestCompletion: true
            }
        };

        #database_connection = new Connection(config);
        #database_connected = true;
    }
}";

        public ConnectionSegement(int indentCount, Sql sql)
            : base(indentCount)
        {
            Name = $"{sql.DbName}_connection";
            GlobalVars.Add($"var {sql.DbName}_connection;");
            GlobalVars.Add($"var {sql.DbName}_connected = false;");
            Requires.Add("var TYPES = require('tedious').TYPES;");
            DependsOnModules.Add("\"tedious\": \"5.0.3\"");

            Methods.Add(connect
                .Replace("#username", sql.Username)
                .Replace("#password", sql.Password)
                .Replace("#connectinString", sql.ConnectinString)
                .Replace("#database", sql.DbName));
        }
    }
}
