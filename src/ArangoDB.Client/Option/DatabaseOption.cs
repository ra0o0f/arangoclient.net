using System;
using System.Collections.Generic;
using System.Text;

namespace ArangoDB.Client.Option
{
    public class DatabaseOption
    {
        public bool WaitForSync { get; set; }

        public bool ThrowForServerErrors { get; set; }

        public bool DisableChangeTracking { get; set; }
    }
}
