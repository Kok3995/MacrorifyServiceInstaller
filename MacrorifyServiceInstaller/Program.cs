using SharpAdbClient;
using System;
using System.Linq;

namespace MacrorifyServiceInstaller
{
    class Program
    {
        static void Main(string[] args)
        {
            Helper.CenterText("Macrorify Native Service Installer");

            var type = ConnectDevices();

            var devices = AdbHelper.GetClient().GetDevices();

            DeviceData device;

            if (devices.Count == 0)
            {
                Console.WriteLine("No Device Connected");
                Pause();
                return;
            }

            if (devices.Count == 1)
                device = devices[0];
            else
            {
                Console.WriteLine("Multiple Connected Devices (" + devices.Count + ")");

                for (int i = 0; i < devices.Count; i++)
                {
                    var d = devices[i];
                    Console.WriteLine((i + 1) + ". " + d.Name);
                }

                int selected = Helper.GetUserInputInt("Please select the device to install: ", 1, devices.Count);
                device = devices[selected - 1];
            }

            Install(device, type);
        }

        public static DeviceType ConnectDevices()
        {
            var supportedDevice = Enum.GetValues(typeof(DeviceType)).Cast<DeviceType>().ToList();

            foreach (var d in supportedDevice)
            {
                Console.WriteLine((int)d + ". " + d.ToString());
            }

            DeviceType selected = (DeviceType)Helper.GetUserInputInt("Please select your device type: ", (int)supportedDevice.First(), (int)supportedDevice.Last());

            IConnector connector;
            switch (selected)
            {
                case DeviceType.Real:
                    connector = new ConnecterRealDevice();
                    break;
                case DeviceType.Nox:
                    connector = new ConnecterNox();
                    break;
                case DeviceType.MEmu:
                    connector = new ConnecterMEmu();
                    break;
                default:
                    throw new NotImplementedException("Unsupported Device Type");
            }

            AdbHelper.StartServer(selected);
            connector.Connect();
            return selected;
        }

        public static void Install(DeviceData device, DeviceType deviceType)
        {
            var abi = AdbHelper.GetArchitecture(device);
            var source = Helper.GetServicePath(abi);

            //push service to device
            AdbHelper.Push(device, source, Constant.SERVICE_DEVICE_PATH, 777);

            //run
            if (AdbHelper.RunService(device, deviceType))
                Console.WriteLine("Success");
            else Console.WriteLine("Fail");

            Pause();
        }

        public static void Pause()
        {
            Console.Write("Press any key to continue... ");
            Console.ReadKey(true);
        }
    }
}
