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
            AdbHelper.GetClient().Connect(new System.Net.DnsEndPoint(Constant.LOCALHOST, Constant.BLUE_PORT));
        }
    }
}
