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
            for (var i = 0; i < 5; i += 1)
            {
                try
                {
                    AdbHelper.GetClient().Connect(new System.Net.DnsEndPoint(Constant.LOCALHOST, Constant.BLUE_PORT + i));
                }
                catch { /*ignored*/ }
            }

            for (var i = 10; i <= 100; i += 10)
            {             
                try
                {
                    AdbHelper.GetClient().Connect(new System.Net.DnsEndPoint(Constant.LOCALHOST, Constant.BLUE_PORT + i));
                } catch { /*ignored*/ }
            }
        }
    }
}
