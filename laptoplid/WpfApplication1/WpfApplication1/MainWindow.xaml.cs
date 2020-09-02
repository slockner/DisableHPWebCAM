
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using HardwareHelperLib;
using System.Collections.Generic;
using System.Management;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using System.Windows.Threading;


namespace WpfApplication1
{




    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HH_Lib hwh = new HH_Lib();
        List<DEVICE_INFO> HardwareList;
        StreamWriter sw = new StreamWriter("C:\\temp\\Test1.log", true);


        [DllImport(@"User32", SetLastError = true, EntryPoint = "RegisterPowerSettingNotification",
            CallingConvention = CallingConvention.StdCall)]

        private static extern IntPtr RegisterPowerSettingNotification(IntPtr hRecipient, ref Guid PowerSettingGuid,
            Int32 Flags);

        internal struct POWERBROADCAST_SETTING
        {
            public Guid PowerSetting;
            public uint DataLength;
            public byte Data;
        }

        Guid GUID_LIDSWITCH_STATE_CHANGE = new Guid(0xBA3E0F4D, 0xB817, 0x4094, 0xA2, 0xD1, 0xD5, 0x63, 0x79, 0xE6, 0xA0, 0xF3);
        const int DEVICE_NOTIFY_WINDOW_HANDLE = 0x00000000;
        const int WM_POWERBROADCAST = 0x0218;
        const int PBT_POWERSETTINGCHANGE = 0x8013;

        private bool? _previousLidState = null;

        public MainWindow()
        {
            HardwareList = hwh.GetAll();
            string Computerstype = GetCurrentChassisType();
            if ((Computerstype == "Notebook") || (Computerstype == "Laptop") || (Computerstype == "Sub-Notebook") || (Computerstype == "Portable"))
            {
                Debug.WriteLine("inLoop", DateTime.Now);
                InitializeComponent();
                this.SourceInitialized += MainWindow_SourceInitialized;












            }


        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            StartUPverification();



            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(5);
            timer.Tick += timer_Tick;
            timer.Start();
        }
        void timer_Tick(object sender, EventArgs e)
        {
            StartUPverification();
        }








        public void StartUPverification()
        {


            checkedlaptoplid();
        }
        private void checkedlaptoplid()
        {

            HardwareList = hwh.GetAll();
            var laptop = GetLaptopModel();
            sw.WriteLine(laptop);
            int[] screensize = GetLaptopModelsize(laptop);
            sw.WriteLine("[{0}]", string.Join(", ", screensize));
            Debug.WriteLine("[{0}]", string.Join(", ", screensize));
            GetLaptopModelsize(screensize);
            sw.Flush();

        }

        public string GetCurrentChassisType()
        {
            ManagementClass systemEnclosures = new ManagementClass("Win32_SystemEnclosure");
            string strType = "Unknown";
            foreach (ManagementObject obj in systemEnclosures.GetInstances())
            {
                foreach (int i in (UInt16[])(obj["ChassisTypes"]))
                {
                    switch (i)
                    {
                        case 1:
                            strType = "Other";
                            break;
                        case 2:
                            strType = "Unknown";
                            break;
                        case 3:
                            strType = "Desktop";
                            break;
                        case 4:
                            strType = "Low Profile Desktop";
                            break;
                        case 5:
                            strType = "Pizza Box";
                            break;
                        case 6:
                            strType = "Mini Tower";
                            break;
                        case 7:
                            strType = "Tower";
                            break;
                        case 8:
                            strType = "Portable";
                            break;
                        case 9:
                            strType = "Laptop";
                            break;
                        case 10:
                            strType = "Notebook";
                            break;
                        case 11:
                            strType = "Handheld";
                            break;
                        case 12:
                            strType = "Docking Station";
                            break;
                        case 13:
                            strType = "All-in-One";
                            break;
                        case 14:
                            strType = "Sub-Notebook";
                            break;
                        case 15:
                            strType = "Space Saving";
                            break;
                        case 16:
                            strType = "Lunch Box";
                            break;
                        case 17:
                            strType = "Main System Chassis";
                            break;
                        case 18:
                            strType = "Expansion Chassis";
                            break;
                        case 19:
                            strType = "Sub-Chassis";
                            break;
                        case 20:
                            strType = "Bus Expansion Chassis";
                            break;
                        case 21:
                            strType = "Peripheral Chassis";
                            break;
                        case 22:
                            strType = "Storage Chassis";
                            break;
                        case 23:
                            strType = "Rack Mount Chassis";
                            break;
                        case 24:
                            strType = "Sealed-Case PC";
                            break;
                        default:
                            strType = "Handheld";
                            break;




                    }


                }
            }

            return strType;
        }








        private string GetLaptopModel()
        {

            var osQuery = new SelectQuery("Win32_ComputerSystemProduct");
            var mgmtScope = new ManagementScope("\\\\.\\root\\cimv2");
            mgmtScope.Connect();
            var mgmtSrchr = new ManagementObjectSearcher(mgmtScope, osQuery);
            foreach (var os in mgmtSrchr.Get())
            {
                var LaptopModel = os.GetPropertyValue("Name").ToString();
                if (!string.IsNullOrEmpty(LaptopModel))
                    return LaptopModel;
                else
                    return "Unknown";
            }
            return "Unknown";
        }

        private static int[] GetLaptopModelsize(string LaptopModel)
        {

            int[] size = { 0, 0 };
            switch (LaptopModel)
            {
                case "HP ZBook 15 G3":
                    size[0] = 34;
                    size[1] = 19;
                    break;
                case "HP EliteBook 840 G5":
                    size[0] = 31;
                    size[1] = 17;
                    break;
                case "HP EliteBook x360 1030 G3":
                    size[0] = 29;
                    size[1] = 17;
                    break;
                case "HP EliteBook x360 830 G6":
                    size[0] = 29;
                    size[1] = 17;
                    break;

            }
            return size;
        }


        private void GetLaptopModelsize(int[] screensize)
        {
            string NamespacePath = "\\\\.\\ROOT\\WMI";
            string ClassName = "WmiMonitorBasicDisplayParams";

            //Create ManagementClass
            ManagementClass oClass = new ManagementClass(NamespacePath + ":" + ClassName);
            bool lidclosed = false;

            foreach (ManagementObject oObject in oClass.GetInstances())
            {
                var MaxHorizontalImageSize = oObject["MaxHorizontalImageSize"].ToString();
                var MaxVerticalImageSize = oObject["MaxVerticalImageSize"].ToString();

                string hor = screensize[0].ToString();
                string ver = screensize[1].ToString();

                sw.WriteLine("*******************");
                sw.WriteLine(hor, DateTime.Now);
                sw.WriteLine(MaxHorizontalImageSize);
                Debug.WriteLine("*******************", DateTime.Now);
                Debug.WriteLine(hor, DateTime.Now);
                Debug.WriteLine(MaxHorizontalImageSize, DateTime.Now);
                if (hor == MaxHorizontalImageSize)
                {

                    sw.WriteLine("&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");
                    sw.WriteLine(ver);
                    sw.WriteLine(MaxVerticalImageSize);
                    Debug.WriteLine("&&&&&&&&&&&&&&&&&&&&&&&&&&&&&", DateTime.Now);
                    Debug.WriteLine(ver, DateTime.Now);
                    Debug.WriteLine(MaxVerticalImageSize, DateTime.Now);
                    if (ver == MaxVerticalImageSize)
                        lidclosed = true;


                }







            }


            if (lidclosed)
            {

                ;

                LidStatusChanged(true);

                sw.WriteLine("lid open");
                Debug.WriteLine("lid open", DateTime.Now);
            }

            else
            {
                //Do some action on lid open event




                LidStatusChanged(false);
                sw.WriteLine("lid closed");

            }




        }




        void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            RegisterForPowerNotifications();
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(hwnd).AddHook(new HwndSourceHook(WndProc));
        }

        private void RegisterForPowerNotifications()
        {
            IntPtr handle = new WindowInteropHelper(Application.Current.Windows[0]).Handle;
            IntPtr hLIDSWITCHSTATECHANGE = RegisterPowerSettingNotification(handle,
                 ref GUID_LIDSWITCH_STATE_CHANGE,
                 DEVICE_NOTIFY_WINDOW_HANDLE);
        }

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_POWERBROADCAST:
                    OnPowerBroadcast(wParam, lParam);
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;
        }

        private void OnPowerBroadcast(IntPtr wParam, IntPtr lParam)
        {
            if ((int)wParam == PBT_POWERSETTINGCHANGE)
            {
                POWERBROADCAST_SETTING ps = (POWERBROADCAST_SETTING)Marshal.PtrToStructure(lParam, typeof(POWERBROADCAST_SETTING));

                if (ps.PowerSetting == GUID_LIDSWITCH_STATE_CHANGE)
                {
                    bool isLidOpen = ps.Data != 0;

                    if (!isLidOpen == _previousLidState)
                    {
                        LidStatusChanged(isLidOpen);
                    }

                    _previousLidState = isLidOpen;
                }
            }
        }


        private void devcon(string HDid, bool process)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.Verb = "runas";
            p.StartInfo.CreateNoWindow = true;
           
            
            string action= "";
            if (process == true)
                 action = "Enable";
            else
                 action = "Disable";
            Debug.WriteLine(" /c c:\\\"Program Files\"\\ADPLaptopCameraHelper\\devcon.exe /" + action + "  \"" + HDid + "\"");
            p.StartInfo.Arguments = " /c c:\\\"Program Files\"\\ADPLaptopCameraHelper\\devcon.exe /" + action +"  \""+ HDid+"\"";
            p.Start();



        }



        private void LidStatusChanged(bool isLidOpen)
        {
            if (isLidOpen)
            {
                //Do some action on lid open event

                string[] devices = new string[2];
                devices[0] = "HP HD Camera";
                devices[1] = "HP Universal Camera Driver";



                foreach (var device in HardwareList)
                {

                    if (device.name.ToString().ToLower().Contains(devices[0].ToLower()))
                    {
                        devcon(device.hardwareId, true);
                        
                        
                        
                        
                        //hwh.SetDeviceState(device, true);
                    }
                    if (device.name.ToString().ToLower().Contains(devices[1].ToLower()))
                    {
                        devcon(device.hardwareId, true);
                        //hwh.SetDeviceState(device, true);
                    }


                }






                Debug.WriteLine("{0}: Lid opened!", DateTime.Now);


            }
            else
            {
                //Do some action on lid close event

                string[] devices = new string[2];
                devices[0] = "HP HD Camera";
                devices[1] = "HP Universal Camera Driver";


                foreach (var device in HardwareList)
                {

                    if (device.name.ToString().ToLower().Contains(devices[0].ToLower()))
                    {
                        devcon(device.hardwareId, false);
                        //hwh.SetDeviceState(device, false);
                    }

                    if (device.name.ToString().ToLower().Contains(devices[1].ToLower()))
                    {
                        devcon(device.hardwareId, false);
                        // hwh.SetDeviceState(device, false);
                    }


                }



                Debug.WriteLine("{0}: Lid closed!", DateTime.Now);
            }
        }
    }
}
