﻿using ArangoDB.Client.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArangoDB.Client.ServiceProvider
{
    public class ScopeService
    {
        public IDatabaseConfig DatabaseConfig { get; set; }
    }
}
