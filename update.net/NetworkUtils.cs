using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace update.net
{
    public class NetworkUtils
    {
        public static void AssertNetworkIsAvailable()
        {
            if (!(System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()))
            {
                throw new NetworkNotFoundException("Initial network not found");
            }
        }
    }
}
