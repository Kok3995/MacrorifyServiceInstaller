using System;
using System.IO;
using System.Text.RegularExpressions;

namespace MacrorifyServiceInstaller
{
  public class ConnecterBlue : IConnector
  {
    static string BluestacksConfigFilePath => Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"BlueStacks_nxt\bluestacks.conf");
    static Regex AdbPortExtraction = new Regex(@"bst\.instance.(.+)\.adb_port=""(?<adbPort>\d+)""", RegexOptions.IgnoreCase);

    public ConnecterBlue()
    {
    }

    public void Connect()
    {

      if (FindAdbPortInConfig(out int bluePort))
      {
        if (TryConnectAdbClient(bluePort))
        {
          return;
        }
      }

      for (var i = 0; i < 5; i += 1)
      {
        if (TryConnectAdbClient(Constant.BLUE_PORT + i))
        {
          return;
        }
      }

      for (var i = 10; i <= 100; i += 10)
      {
        if (TryConnectAdbClient(Constant.BLUE_PORT + i))
        {
          return;
        }
      }
    }

    private static bool TryConnectAdbClient(int port)
    {
      try
      {
        AdbHelper.GetClient().Connect(new System.Net.DnsEndPoint(Constant.LOCALHOST, port));
        return true;
      }
      catch { /*ignored*/ }

      return false;
    }

    private bool FindAdbPortInConfig(out int adbPort)
    {
      adbPort = Constant.BLUE_PORT;
      if (!File.Exists(BluestacksConfigFilePath))
        return false;

      try
      {
        foreach (var line in File.ReadAllLines(BluestacksConfigFilePath))
        {
          // match any Android version by pattern of "bst.instance.(.*).adb_port="5555""
          var match = AdbPortExtraction.Match(line);
          if (match.Success)
          {
            adbPort = Convert.ToInt32(match.Groups["adbPort"].Value);
            return true;
          }
        }
      }
      catch { /*ignored*/ }

      return false;
    }
  }
}
