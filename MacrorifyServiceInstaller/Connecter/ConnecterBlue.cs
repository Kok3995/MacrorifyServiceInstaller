using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace MacrorifyServiceInstaller
{
    public class ConnecterBlue : IConnector
    {
        public ConnecterBlue()
        {
        }

        public void Connect()
        {
            for (var i = 0; i <= 100; i += 10)
            {             
                try
                {
                    AdbHelper.GetClient().Connect(new System.Net.DnsEndPoint(Constant.LOCALHOST, Constant.BLUE_PORT + i));
                } catch { /*ignored*/ }

                if (i == 0)
                {
                    try
                    {
                        //Also connect port 5556
                        AdbHelper.GetClient().Connect(new System.Net.DnsEndPoint(Constant.LOCALHOST, Constant.BLUE_PORT + 1));
                    }
                    catch { /*ignored*/ }
                }
            }
        }
    }
}
