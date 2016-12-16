using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace update.net
{
    public static class StringExtensions
    {
        public static void AssertNotNullNotEmpty(this string str, string name)
        {
            if (String.IsNullOrEmpty(str))
            {
                throw new ArgumentNullException(name);
            }
        }

    }
}
