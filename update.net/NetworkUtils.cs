using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace update.net
{
    /// <summary>
    /// A class holding many utility functions pertaining to network operations.
    /// </summary>
    public class NetworkUtils
    {
        /// <summary>
        /// Checks if the network interface shows a network available. If not, 
        /// throws a NetworkNotFoundException
        /// </summary>
        /// <throws>NetworkNotFoundException if no network is available</throws>
        public static void AssertNetworkIsAvailable()
        {
            if (!(System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()))
            {
                throw new NetworkNotFoundException("Initial network not found");
            }
        }
    }
}
