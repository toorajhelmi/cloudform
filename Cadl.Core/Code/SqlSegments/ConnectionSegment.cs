using System;
using System.Collections.Generic;
using Cadl.Core.Components;
using Cadl.Core.Interpreters;

namespace Cadl.Core.Code.SqlSegments
{
    public class ConnectionSegement : Segment
    {
        private const string connect = @"
function connectTo#database() {
    if (#database_connection != null) {
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
    }
}";

        public ConnectionSegement(int indentCount, Sql sql)
            : base(indentCount)
        {
            Name = $"{sql.DbName}_connection";
            GlobalVars.Add($"var {sql.DbName}_connection;");
            Requires.Add("var Connection = require(\"tedious\").Connection;");
            Methods.Add(connect
                .Replace("#username", sql.Username)
                .Replace("#password", sql.Password)
                .Replace("#connectinString", sql.ConnectinString)
                .Replace("#database", sql.DbName));
        }
    }
}
