using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace update.net
{
    /// <summary>
    /// A class holding string extension methods for strings to call directly
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Asserts that a string is not null or empty, throwing if they are.
        /// </summary>
        /// <param name="str">The string to check for nullity/emptiness</param>
        /// <param name="name">The name of the string to display in the error</param>
        /// <throws>ArgumentNullException if the string is null or empty</throws>
        public static void AssertNotNullNotEmpty(this string str, string name)
        {
            if (String.IsNullOrEmpty(str))
            {
                throw new ArgumentNullException(name);
            }
        }

    }
}
