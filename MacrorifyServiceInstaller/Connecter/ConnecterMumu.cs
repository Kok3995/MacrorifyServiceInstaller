using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace MacrorifyServiceInstaller
{
    public class ConnecterMumu : IConnector
    {
        public ConnecterMumu()
        {
        }

        public void Connect()
        {
            AdbHelper.GetClient().Connect(new System.Net.DnsEndPoint(Constant.LOCALHOST, Constant.MUMU_PORT));
        }
    }
}
