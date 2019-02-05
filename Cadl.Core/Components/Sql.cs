using System;
using System.Collections.Generic;

namespace Cadl.Core.Components
{
    public class Sql : Component
    {
        public string DbName { get; set; }
        public List<Table> Tables { get; set; } = new List<Table>();
    }
}
