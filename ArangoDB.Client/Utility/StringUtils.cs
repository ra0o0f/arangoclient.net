using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Utility
{
    public static class StringUtils
    {
        public static string ToCamelCase(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            if (!char.IsUpper(s[0]))
                return s;

            char[] chars = s.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                bool hasNext = (i + 1 < chars.Length);
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                    break;

#if !(NETFX_CORE || PORTABLE)
                chars[i] = char.ToLower(chars[i], CultureInfo.InvariantCulture);
#else
                chars[i] = char.ToLowerInvariant(chars[i]);
#endif
            }

            return new string(chars);
        }
    }
}
