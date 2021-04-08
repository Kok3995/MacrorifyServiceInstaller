using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace MacrorifyServiceInstaller
{
    public class AdbHelper
    {
        private static AdbClient adbClient;

        public static AdbClient GetClient()
        {
            if (adbClient == null)
            {
                adbClient = new AdbClient();
            }

            return adbClient;
        }

        internal static void StartServer(DeviceType deviceType)
        {
            new AdbServer().StartServer(Helper.GetAdbExePath(deviceType), false);
        }

        internal static string GetArchitecture(DeviceData device)
        {
            var receiver = new ConsoleOutputReceiver();

            GetClient().ExecuteRemoteCommand(@"getprop ro.product.cpu.abi", device, receiver);

            return receiver.ToString().TrimEnd('\r', '\n');
        }

        internal static void Push(DeviceData device, string source, string target, int permission)
        {            
            using (SyncService service = new SyncService(new AdbSocket(new IPEndPoint(IPAddress.Loopback, AdbClient.AdbServerPort)), device))
            using (Stream stream = File.OpenRead(source))
            {
                service.Push(stream, target, permission, DateTime.Now, null, CancellationToken.None);
            }
        }

        internal static bool IsServiceRunning(DeviceData device)
        {
            var receiver = new ConsoleOutputReceiver();

            GetClient().ExecuteRemoteCommand(@"ps", device, receiver);

            return receiver.ToString().Contains("minitouch");
        }

        internal static bool RunService(DeviceData device, DeviceType deviceType)
        {
            if (IsServiceRunning(device))
                return true;

            RunCommand(@"""" + Helper.GetAdbExePath(deviceType) + @"""" + @" -s " + device.Serial + @" shell ""/data/local/tmp/minitouch &""");

            //Wait for the executable to run
            Thread.Sleep(1000);

            return IsServiceRunning(device);
        }

        internal static void RunCommand(string command)
        {
            using (Process proc = new Process())
            {
                ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd.exe");
                procStartInfo.RedirectStandardInput = true;
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = true;
                proc.StartInfo = procStartInfo;
                proc.Start();
                proc.StandardInput.WriteLine(command);
            }
        }
    }
}
