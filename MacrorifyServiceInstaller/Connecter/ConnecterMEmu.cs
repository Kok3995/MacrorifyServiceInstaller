using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace MacrorifyServiceInstaller
{
    public class ConnecterMEmu : IConnector
    {
        public ConnecterMEmu()
        {
        }

        public void Connect()
        {
            AdbHelper.GetClient().Connect(new System.Net.DnsEndPoint(Constant.LOCALHOST, Constant.MEMU_PORT));
        }
    }
}
