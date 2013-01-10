using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;

namespace Weavver.Net
{
     public class Helper
     {
          public static bool IsPortAvailable(int port)
          {
               IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
               System.Net.IPEndPoint[] endpoints = ipGlobalProperties.GetActiveTcpListeners();

               foreach (System.Net.IPEndPoint tcpi in endpoints)
               {
                    if (tcpi.Port ==port)
                    {
                         return false;
                    }
               }
               return true;
          }
     }
}
