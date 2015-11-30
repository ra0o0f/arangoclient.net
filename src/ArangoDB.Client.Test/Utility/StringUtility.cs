using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Test.Utility
{
    public static class StringUtility
    {
        public static string RemoveSpaces(this string s)
        {
            return s.Trim().Replace("\r", " ").Replace("\n", " ")
                .Replace("    ", " ").Replace("   ", " ").Replace("  ", " ");
        }
    }
}
