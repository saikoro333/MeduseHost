using System;
using System.Collections.Generic;
using System.Text;

using vJoyInterfaceWrap;

using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using System.Management;
using System.IO.Ports;

//using Vortice.XInput;
//using SharpDX.DirectInput;

//https://github.com/dotnet/runtime/issues/28840
//https://qastack.jp/programming/3331043/get-list-of-connected-usb-devices

namespace WinFormsApp1
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        /// 
        static public vJoy joystick;
        static public vJoy.JoystickState iReport;
        static public uint id = 1;

        [STAThread]
        static void Main(string[] args)
        {
            //ApplicationConfiguration.Initialize();
            // Application.Run(new Form1());
            joystick = new vJoy();
            iReport = new vJoy.JoystickState();
            Form2 f2 = new Form2();
            f2.Show();
            //Task t1 = new Task(f2.Show);
            //t1.Start();

            /*
            var dev = DriveInfo.GetDrives();
            foreach (var d in dev)
            {
                MessageBox.Show(d.ToString());
            }
            */
            ManagementObjectCollection collection;

            //SelectQuery q = new SelectQuery(@"Win32_USBHub", @"State='Running'");
            //using (var searcher = new ManagementObjectSearcher(@"SELECT * FROM Win32_USBHub")) collection = searcher.Get();
            using (var searcher = new ManagementObjectSearcher(@"SELECT * FROM Win32_PnPEntity where DeviceID Like ""HID%""")) collection = searcher.Get();
            //using (var searcher = new ManagementObjectSearcher(@"SELECT * FROM Win32_SerialPort")) collection = searcher.Get();

            // �擾����USBHub��񂩂�AID�������o���A�f�o�C�X���X�g�ɒǉ�

            int icnt = 0;
            string portnum="";
            foreach (var device in collection)
            {
                //MessageBox.Show(device.GetPropertyValue("DeviceID").ToString());
                //MessageBox.Show(device.GetPropertyValue("Name")+" , " + device.GetPropertyValue("PNPDeviceID"));
                icnt++;
                if(icnt == 1)
                {
                    portnum = device.GetPropertyValue("DeviceID").ToString();
                }
            }
            collection.Dispose();
            MessageBox.Show(icnt + "  l   " + portnum);

            //var serialPort = new SerialPort(portnum, 115200, Parity.None, 8, StopBits.One);
            //serialPort.Open();  // �V���A���|�[�g�I�[�v��


            // Device ID can only be in the range 1-16

            if (args.Length > 0 && !String.IsNullOrEmpty(args[0]))
                id = Convert.ToUInt32(args[0]);











            if (id <= 0 || id > 16)
            {
                Console.WriteLine("Illegal device ID {0}\nExit!", id);
                return;
            }

            // Get the driver attributes (Vendor ID, Product ID, Version Number)
            if (!joystick.vJoyEnabled())
            {
                Console.WriteLine("vJoy driver not enabled: Failed Getting vJoy attributes.\n");
                return;
            }
            else
                Console.WriteLine("Vendor: {0}\nProduct :{1}\nVersion Number:{2}\n", joystick.GetvJoyManufacturerString(), joystick.GetvJoyProductString(), joystick.GetvJoySerialNumberString());



            // Get the state of the requested device
            VjdStat status = joystick.GetVJDStatus(id);
            switch (status)
            {
                case VjdStat.VJD_STAT_OWN:
                    //Console.WriteLine("vJoy Device {0} is already owned by this feeder\n", id);
                    MessageBox.Show("own");
                    break;
                case VjdStat.VJD_STAT_FREE:
                    //Console.WriteLine("vJoy Device {0} is free\n", id);
                    MessageBox.Show("free");
                    break;
                case VjdStat.VJD_STAT_BUSY:
                    //Console.WriteLine("vJoy Device {0} is already owned by another feeder\nCannot continue\n", id);
                    MessageBox.Show("busy");
                    return;
                case VjdStat.VJD_STAT_MISS:
                    //Console.WriteLine("vJoy Device {0} is not installed or disabled\nCannot continue\n", id);
                    MessageBox.Show("miss");
                    return;
                default:
                    //Console.WriteLine("vJoy Device {0} general error\nCannot continue\n", id);
                    MessageBox.Show("def");
                    return;
            };

            // Check which axes are supported
            bool AxisX = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_X);
            bool AxisY = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Y);
            bool AxisZ = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Z);
            bool AxisRX = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RX);
            bool AxisRZ = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RZ);
            // Get the number of buttons and POV Hat switchessupported by this vJoy device
            int nButtons = joystick.GetVJDButtonNumber(id);
            int ContPovNumber = joystick.GetVJDContPovNumber(id);
            int DiscPovNumber = joystick.GetVJDDiscPovNumber(id);

            // Print results
            Console.WriteLine("\nvJoy Device {0} capabilities:\n", id);
            Console.WriteLine("Numner of buttons\t\t{0}\n", nButtons);
            Console.WriteLine("Numner of Continuous POVs\t{0}\n", ContPovNumber);
            Console.WriteLine("Numner of Descrete POVs\t\t{0}\n", DiscPovNumber);
            Console.WriteLine("Axis X\t\t{0}\n", AxisX ? "Yes" : "No");
            Console.WriteLine("Axis Y\t\t{0}\n", AxisX ? "Yes" : "No");
            Console.WriteLine("Axis Z\t\t{0}\n", AxisX ? "Yes" : "No");
            Console.WriteLine("Axis Rx\t\t{0}\n", AxisRX ? "Yes" : "No");
            Console.WriteLine("Axis Rz\t\t{0}\n", AxisRZ ? "Yes" : "No");

            // Test if DLL matches the driver
            UInt32 DllVer = 0, DrvVer = 0;
            bool match = joystick.DriverMatch(ref DllVer, ref DrvVer);
            if (match)
                Console.WriteLine("Version of Driver Matches DLL Version ({0:X})\n", DllVer);
            else
                Console.WriteLine("Version of Driver ({0:X}) does NOT match DLL Version ({1:X})\n", DrvVer, DllVer);

            MessageBox.Show(DllVer + "," + DrvVer);

            // Acquire the target
            if ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE) && (!joystick.AcquireVJD(id))))
            {
                Console.WriteLine("Failed to acquire vJoy device number {0}.\n", id);
                return;
            }
            else
                Console.WriteLine("Acquired: vJoy device number {0}.\n", id);

            Console.WriteLine("\npress enter to stat feeding");
            //Console.ReadKey(true);

            int X, Y, Z, ZR, XR;
            uint count = 0;
            long maxval = 0;

            X = 20;
            Y = 30;
            Z = 40;
            XR = 60;
            ZR = 80;

            joystick.GetVJDAxisMax(id, HID_USAGES.HID_USAGE_X, ref maxval);
            MessageBox.Show(maxval + "");
            joystick.GetVJDAxisMin(id, HID_USAGES.HID_USAGE_X, ref maxval);
            MessageBox.Show(maxval + "");
            bool res;
            // Reset this device to default values
            //joystick.ResetVJD(id);
            //joystick.ResetAll();

            //reset button
            for(uint i=0; i<nButtons; i++)
            {
                res = joystick.SetBtn(false, id, 1+i);
            }

            for(uint i=0; i < ContPovNumber; i++)
            {
                res = joystick.SetContPov(0, id, 1+i);
            }
            for (uint i = 0; i < DiscPovNumber; i++)
            {
                res = joystick.SetDiscPov(0, id, 1 + i);
            }













            // Feed the device in endless loop
            while (true)
            {
                // Set position of 4 axes

                res = joystick.SetAxis(X, id, HID_USAGES.HID_USAGE_X);
                //res = joystick.SetAxis(Y, id, HID_USAGES.HID_USAGE_Y);
                //res = joystick.SetAxis(Z, id, HID_USAGES.HID_USAGE_Z);
                //res = joystick.SetAxis(XR, id, HID_USAGES.HID_USAGE_RX);
                //res = joystick.SetAxis(ZR, id, HID_USAGES.HID_USAGE_RZ);

                // Press/Release Buttons
                //res = joystick.SetBtn(true, id, count / 50);
                //res = joystick.SetBtn(false, id, 1 + count / 50);
                //res = joystick.SetBtn(false, id, 5);

                // If Continuous POV hat switches installed - make them go round
                // For high values - put the switches in neutral state
                if (ContPovNumber > 0)
                {
                    if ((count * 70) < 30000)
                    {
                        res = joystick.SetContPov(((int)count * 70), id, 1);
                        res = joystick.SetContPov(((int)count * 70) + 2000, id, 2);
                        //res = joystick.SetContPov(((int)count * 70) + 4000, id, 3);
                        //res = joystick.SetContPov(((int)count * 70) + 6000, id, 4);
                    }
                    else
                    {
                        res = joystick.SetContPov(-1, id, 1);
                        res = joystick.SetContPov(-1, id, 2);
                        //res = joystick.SetContPov(-1, id, 3);
                        //res = joystick.SetContPov(-1, id, 4);
                    };
                };

                // If Discrete POV hat switches installed - make them go round
                // From time to time - put the switches in neutral state
                if (DiscPovNumber > 0)
                {
                    if (count < 550)
                    {
                        joystick.SetDiscPov((((int)count / 20) + 0) % 4, id, 1);
                        joystick.SetDiscPov((((int)count / 20) + 1) % 4, id, 2);
                        //joystick.SetDiscPov((((int)count / 20) + 2) % 4, id, 3);
                        //joystick.SetDiscPov((((int)count / 20) + 3) % 4, id, 4);
                    }
                    else
                    {
                        joystick.SetDiscPov(-1, id, 1);
                        joystick.SetDiscPov(-1, id, 2);
                        //joystick.SetDiscPov(-1, id, 3);
                        //joystick.SetDiscPov(-1, id, 4);
                    };
                };


                if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                    res = joystick.SetBtn(true, id, 5);
                else
                    res = joystick.SetBtn(false, id, 5);

                if (count % 100 == 0)
                {
                    res = joystick.SetBtn(true, id, 1);
                }
                else
                {
                    res = joystick.SetBtn(false, id, 1);
                }



                System.Threading.Thread.Sleep(20);
                X += 150; if (X > maxval) X = 0;
                Y += 250; if (Y > maxval) Y = 0;
                Z += 350; if (Z > maxval) Z = 0;
                XR += 220; if (XR > maxval) XR = 0;
                ZR += 200; if (ZR > maxval) ZR = 0;
                count++;

                if (count > 640)
                    count = 0;

                Application.DoEvents();

            } // While (Robust)



        }

    }

}