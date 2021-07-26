using SharpAdbClient;
using System;
using System.Linq;

namespace MacrorifyServiceInstaller
{
    class Program
    {
        static bool isDebug;

        static void Main()
        {
            string[] args = Environment.GetCommandLineArgs().Skip(1).ToArray();
            Func<string, string> lookupFunc =
                option => args.Where(s => s.StartsWith(option)).Select(s => s.Substring(option.Length)).FirstOrDefault();

            string debugOption = lookupFunc("debug=");

            isDebug = debugOption == "1";

            Helper.CenterText("Macrorify Native Service Installer v1.1.1");

            DeviceType type = DeviceType.Real;

            try
            {
                type = ConnectDevices();
            }
            catch (Exception ex)
            {
                if (isDebug)
                    Console.WriteLine(ex);

                Pause();
                return;
            }

            var devices = AdbHelper.GetClient().GetDevices().Where(d => d.State == DeviceState.Online).ToList();

            DeviceData[] selectedDevices;

            if (devices.Count == 0)
            {
                Console.WriteLine("No Device Connected");
                Pause();
                return;
            }

            if (devices.Count == 1)
                selectedDevices = new DeviceData[] { devices[0] };
            else
            {
                Console.WriteLine("\nMultiple Connected Devices (" + devices.Count + ")");

                Console.WriteLine("0. All");

                for (int i = 0; i < devices.Count; i++)
                {
                    var d = devices[i];
                    Console.WriteLine((i + 1) + ". " + GetDeviceDisplayName(d));
                }

                int selected = Helper.GetUserInputInt("\nPlease select the device to install: ", 0, devices.Count);
                if (selected == 0)
                    selectedDevices = devices.ToArray();
                else
                    selectedDevices = new DeviceData[] { devices[selected - 1] };
            }

            foreach (var device in selectedDevices)
            {
                try
                {

                    Console.WriteLine(GetDeviceDisplayName(device) + " - Installing");
                    Install(device, type);
                } 
                catch (Exception ex)
                {
                    if (isDebug)
                        Console.WriteLine(ex);

                    Console.WriteLine(GetDeviceDisplayName(device) + " - Error");
                };
            }

            Pause();
        }

        public static DeviceType ConnectDevices()
        {
            var supportedDevice = Enum.GetValues(typeof(DeviceType)).Cast<DeviceType>().ToList();

            foreach (var d in supportedDevice)
            {
                Console.WriteLine((int)d + ". " + d.ToString());
            }

            DeviceType selected = (DeviceType)Helper.GetUserInputInt("\nPlease select your device type: ", (int)supportedDevice.First(), (int)supportedDevice.Last());

            IConnector connector;
            switch (selected)
            {
                case DeviceType.Real:
                case DeviceType.Nox:
                case DeviceType.LDPlayer:
                    connector = new ConnecterNone();
                    break;
                case DeviceType.MEmu:
                    connector = new ConnecterMEmu();
                    break;
                case DeviceType.BlueStack:
                    connector = new ConnecterBlue();
                    break;
                case DeviceType.MuMu:
                    connector = new ConnecterMumu();
                    break;
                default:
                    throw new NotImplementedException("Unsupported Device Type");
            }

            Console.WriteLine("Start Adb Server");
            AdbHelper.StartServer(selected);

            try
            {
                connector.Connect();
            }
            catch (Exception ex)
            {
                if (isDebug)
                    Console.WriteLine(ex);

                Console.WriteLine($"Error when trying to connect to {selected}");
                throw new Exception();
            }

            return selected;
        }

        public static void Install(DeviceData device, DeviceType deviceType)
        {
            if (device.State == DeviceState.Offline)
            {
                Console.WriteLine(GetDeviceDisplayName(device) + " - Device is Offline");
                return;
            }
    
            var abi = AdbHelper.GetArchitecture(device);
            Console.WriteLine(GetDeviceDisplayName(device) + " - Device Architecture: " + abi);

            var source = Helper.GetServicePath(abi);

            //push service to device
            Console.WriteLine(GetDeviceDisplayName(device) + " - Push Service to Device");
            AdbHelper.Push(device, source, Constant.SERVICE_DEVICE_PATH, 777);

            //run
            Console.WriteLine(GetDeviceDisplayName(device) + " - Run Service");
            if (AdbHelper.RunService(device, deviceType))
                Console.WriteLine(GetDeviceDisplayName(device) + " - Success");
            else
            {
                Console.WriteLine(GetDeviceDisplayName(device) + " - Fail");

                if (deviceType == DeviceType.LDPlayer || deviceType == DeviceType.BlueStack)
                    Console.WriteLine("BlueStack or LDPlayer need to enable ADB in their Setting. Check the tutorial");
            }
        }

        public static void Pause()
        {
            Console.Write("Press any key to continue... ");
            Console.ReadKey(true);
        }

        private static string GetDeviceDisplayName(DeviceData device)
        {
            return $"{device.Model} ({device.Name}) Serial: {device.Serial} Status: {device.State}";
        }
    }
}
