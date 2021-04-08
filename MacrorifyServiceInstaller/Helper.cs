using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MacrorifyServiceInstaller
{
    class Helper
    {
        public static void CenterText(string text)
        {
            Console.Write(new string(' ', (Console.WindowWidth - text.Length) / 2));
            Console.WriteLine(text);
        }

        public static string GetAdbExePath(DeviceType deviceType)
        {
            switch (deviceType)
            {
                case DeviceType.Real:
                case DeviceType.MEmu: return Path.Combine(Environment.CurrentDirectory, "adb", "adb.exe");
                case DeviceType.Nox: return Path.Combine(Environment.CurrentDirectory, "adb", "nox", "adb.exe");
                default:
                    throw new NotImplementedException();
            }
        }

        public static int GetUserInputInt(string message, int min, int max)
        {
            do
            {
                Console.Write(message);
                var selectedString = Console.ReadLine();
                if (!int.TryParse(selectedString, out int selected) || selected < min || selected > max)
                    continue;

                return selected;
            } while (true);
        }

        public static string GetServicePath(string abi)
        {
            return Path.Combine(Environment.CurrentDirectory, "Service", abi, "minitouch");
        }
    }
}
