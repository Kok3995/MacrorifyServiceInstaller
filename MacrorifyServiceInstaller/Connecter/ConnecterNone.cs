using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace MacrorifyServiceInstaller
{
    public class ConnecterNone : IConnector
    {
        public ConnecterNone()
        {
        }

        public void Connect()
        {
            /* No need to do anything */
        }
    }
}
