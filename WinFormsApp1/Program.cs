using System;
using System.Collections.Generic;
using System.Text;

using vJoyInterfaceWrap;
using System.Windows.Input;

using MessageBox = System.Windows.Forms.MessageBox;



//https://github.com/dotnet/runtime/issues/28840
//https://qastack.jp/programming/3331043/get-list-of-connected-usb-devices

namespace WinFormsApp1
{
    internal static class Program
    {
        const int MAX_PLAYER_NUM = 3;
        static public PlayerController[] players = new PlayerController[MAX_PLAYER_NUM];
        static int CurrEntryCount = 0;

        static MainFrom f2;
        [STAThread]
        static void Main(string[] args)
        {

            HostControl hostControl = new HostControl();
            hostControl.Initial();
            //f2 = new Form2();
            //f2.Show();
     
            // Device ID can only be in the range 1-16

            /*
            if (args.Length > 0 && !String.IsNullOrEmpty(args[0]))
                id = Convert.ToUInt32(args[0]);

            */
            //setPlayer();
            //setupVjoyController();

            //setPreLoop();
            //InputLoop();

 

        }

        /*static void setPlayer()
        {
            for(uint i = 0; i < MAX_PLAYER_NUM; i++)
            {
                players[i] = new PlayerController(i + 1,WiiController.PLATFORM);
                players[i].joystick = new vJoy();
            }
        }
        */
        static void setupVjoyController()
        {
            for (uint i = 0; i < MAX_PLAYER_NUM; i++)
            {
                setupVjoyController(players[i]);
            }
        }
        static void setPreLoop()
        {
            for(uint i = 0; i < MAX_PLAYER_NUM; i++)
            {
                players[i].preEnterLoop();
            }
        }

        static private void InputLoop()
        {
            while (true)
            {

                for (int i = 0; i < MAX_PLAYER_NUM; i++)
                {
                    int keyInput = getKeybordInput();
                    players[i].setButtonInput(keyInput);
                    setVJoyInput(players[i]);
                    String str = String.Format("PlayerNum: {0}\r\nInput {1}"
                        , i + 1, players[i].InputToString());
                    //f2.setButtonText(i, str);

                }
                System.Threading.Thread.Sleep(15);
                System.Windows.Forms.Application.DoEvents();
            }
        }

        static private int getKeybordInput()
        {
            int key = 0;

            if (Keyboard.IsKeyDown(Key.W) || Keyboard.IsKeyDown(Key.Up))
            {
                key |= (int)WiiController.ButtonBit.UP;
            }
            if (Keyboard.IsKeyDown(Key.S) || Keyboard.IsKeyDown(Key.Down))
            {
                key |= (int)WiiController.ButtonBit.DOWN;
            }
            if (Keyboard.IsKeyDown(Key.A) || Keyboard.IsKeyDown(Key.Left))
            {
                key |= (int)WiiController.ButtonBit.LEFT;
            }
            if (Keyboard.IsKeyDown(Key.D) || Keyboard.IsKeyDown(Key.Right))
            {
                key |= (int)WiiController.ButtonBit.RIGHT;
            }
            if (Keyboard.IsKeyDown(Key.Space) == true)
            {
                key |= (int)WiiController.ButtonBit.TWO;
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) == true)
            {
                key |= (int)WiiController.ButtonBit.ONE;
            }

            if (Keyboard.IsKeyDown(Key.D1) == true)
            {
                key |= (int)WiiController.ButtonBit.A;
            }
            if (Keyboard.IsKeyDown(Key.D2) == true)
            {
                key |= (int)WiiController.ButtonBit.B;
            }
            if (Keyboard.IsKeyDown(Key.Z) == true)
            {
                key |= (int)WiiController.ButtonBit.SHAKE;
            }
            return key;
        }

        /*
        static private void setVJoyInput(PlayerController player)
        {
            bool res;
            //button
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                res = player.joystick.SetBtn(true, player.id, 5);
            else
                res = player.joystick.SetBtn(false, player.id, 5);

            if (player.count % 50 == 0)
            {
                res = player.joystick.SetBtn(true, player.id, 1);
            }
            else
            {
                res = player.joystick.SetBtn(false, player.id, 1);
            }

            //analog stick
            player.X += 150; if (player.X > player.maxval) player.X = 0;
            player.Y += 250; if (player.Y > player.maxval) player.Y = 0;
            player.Z += 350; if (player.Z > player.maxval) player.Z = 0;
            player.XR += 220; if (player.XR > player.maxval) player.XR = 0;
            player.ZR += 200; if (player.ZR > player.maxval) player.ZR = 0;
            player.count++;

            //if (player.count > 10000000)
                //player.count = 0;

        }
        */
        static private void setVJoyInput(PlayerController player,int key)
        {
            bool res;
            for (int i = 0; i < 16; i++)
            {
                if (((key >> i) & 1) == 1)
                {
                    res = player.joystick.SetBtn(true, player.vjoyDeviceID, (uint)i + 1);
                }
                else
                {
                    res = player.joystick.SetBtn(false, player.vjoyDeviceID, (uint)i + 1);
                }
            }
        }

        static private void setVJoyInput(PlayerController player)
        {
            bool res;
            int key = player.buttons;
            for (int i = 0; i < 16; i++)
            {
                if (((key >> i) & 1) == 1)
                {
                    res = player.joystick.SetBtn(true, player.vjoyDeviceID, (uint)i + 1);
                }
                else
                {
                    res = player.joystick.SetBtn(false, player.vjoyDeviceID, (uint)i + 1);
                }
            }
        }

        static private void setVJoyInput(int key)
        {
            bool res;
            PlayerController player = players[(key & 0xC0000000)>>31];
            for (int i = 0; i < 16; i++)
            {
                if (((key >> i) & 1) == 1)
                {
                    res = player.joystick.SetBtn(true, player.vjoyDeviceID, (uint)i + 1);
                }
                else
                {
                    res = player.joystick.SetBtn(false, player.vjoyDeviceID, (uint)i + 1);
                }
            }

        }

        static private bool setupVjoyController(PlayerController player)
        {

            if (player.vjoyDeviceID <= 0 || player.vjoyDeviceID > 16)
            {
                Console.WriteLine("Illegal device ID {0}\nExit!", player.vjoyDeviceID);
                return false;
            }

            // Get the driver attributes (Vendor ID, Product ID, Version Number)
            if (!player.joystick.vJoyEnabled())
            {
                Console.WriteLine("vJoy driver not enabled: Failed Getting vJoy attributes.\n");
                return false;
            }
            else
                Console.WriteLine("Vendor: {0}\nProduct :{1}\nVersion Number:{2}\n", player.joystick.GetvJoyManufacturerString(), player.joystick.GetvJoyProductString(), player.joystick.GetvJoySerialNumberString());



            GetStatusDevice(player);

            TestIfMatchesDriver(player);

            CheckContRanges(player);

            PrintResult(player);

            AquireTarget(player);

   
            return true;
        }

        static private bool GetStatusDevice(PlayerController player)
        {
            // Get the state of the requested device
            bool isShow = false;
            player.vJoyStatus = player.joystick.GetVJDStatus(player.vjoyDeviceID);
            switch (player.vJoyStatus)
            {
                case VjdStat.VJD_STAT_OWN:
                    Console.WriteLine("vJoy Device {0} is already owned by this feeder\n", player.vjoyDeviceID);
                    if(isShow)MessageBox.Show("own");
                    break;
                case VjdStat.VJD_STAT_FREE:
                    Console.WriteLine("vJoy Device {0} is free\n", player.vjoyDeviceID);
                    if (isShow) MessageBox.Show("free");
                    break;
                case VjdStat.VJD_STAT_BUSY:
                    Console.WriteLine("vJoy Device {0} is already owned by another feeder\nCannot continue\n", player.vjoyDeviceID);
                    MessageBox.Show("busy");
                    return false;
                case VjdStat.VJD_STAT_MISS:
                    Console.WriteLine("vJoy Device {0} is not installed or disabled\nCannot continue\n", player.vjoyDeviceID);
                    MessageBox.Show("miss");
                    return false;
                default:
                    Console.WriteLine("vJoy Device {0} general error\nCannot continue\n", player.vjoyDeviceID);
                    MessageBox.Show("def");
                    return false;
            };
            return true;
        }
        static private void CheckContRanges(PlayerController player)
        {
            // Check which axes are supported
            bool AxisX = player.joystick.GetVJDAxisExist(player.vjoyDeviceID, HID_USAGES.HID_USAGE_X);
            bool AxisY = player.joystick.GetVJDAxisExist(player.vjoyDeviceID, HID_USAGES.HID_USAGE_Y);
            bool AxisZ = player.joystick.GetVJDAxisExist(player.vjoyDeviceID, HID_USAGES.HID_USAGE_Z);
            bool AxisRX = player.joystick.GetVJDAxisExist(player.vjoyDeviceID, HID_USAGES.HID_USAGE_RX);
            bool AxisRZ = player.joystick.GetVJDAxisExist(player.vjoyDeviceID, HID_USAGES.HID_USAGE_RZ);
            player.setCheckAxisData(AxisX, AxisY, AxisZ, AxisRX, AxisRZ);

            // Get the number of buttons and POV Hat switchessupported by this vJoy device
            int nButtons = player.joystick.GetVJDButtonNumber(player.vjoyDeviceID);
            int ContPovNumber = player.joystick.GetVJDContPovNumber(player.vjoyDeviceID);
            int DiscPovNumber = player.joystick.GetVJDDiscPovNumber(player.vjoyDeviceID);
            player.setNumberOfButtons(nButtons, ContPovNumber, DiscPovNumber);
        }

        static private void PrintResult(PlayerController player)
        {
            // Print results
            Console.WriteLine("\nvJoy Device {0} capabilities:\n", player.vjoyDeviceID);
            Console.WriteLine("Numner of buttons\t\t{0}\n", player.nButtons);
            Console.WriteLine("Numner of Continuous POVs\t{0}\n", player.ContPovNumber);
            Console.WriteLine("Numner of Descrete POVs\t\t{0}\n", player.DiscPovNumber);
            Console.WriteLine("Axis X\t\t{0}\n", player.AxisX ? "Yes" : "No");
            Console.WriteLine("Axis Y\t\t{0}\n", player.AxisX ? "Yes" : "No");
            Console.WriteLine("Axis Z\t\t{0}\n", player.AxisX ? "Yes" : "No");
            Console.WriteLine("Axis Rx\t\t{0}\n", player.AxisRX ? "Yes" : "No");
            Console.WriteLine("Axis Rz\t\t{0}\n", player.AxisRZ ? "Yes" : "No");
        }

        static private bool TestIfMatchesDriver(PlayerController player)
        {
            // Test if DLL matches the driver
            UInt32 DllVer = 0, DrvVer = 0;
            bool match = player.joystick.DriverMatch(ref DllVer, ref DrvVer);
            if (match)
            {
                Console.WriteLine("Version of Driver Matches DLL Version ({0:X})\n", DllVer);
                return true;
            }
            else
            {
                Console.WriteLine("Version of Driver ({0:X}) does NOT match DLL Version ({1:X})\n", DrvVer, DllVer);
                return false;
            }
        }

        static private bool AquireTarget(PlayerController player)
        {
            // Acquire the target
            if ((player.vJoyStatus == VjdStat.VJD_STAT_OWN) || ((player.vJoyStatus == VjdStat.VJD_STAT_FREE) && (!player.joystick.AcquireVJD(player.vjoyDeviceID))))
            {
                Console.WriteLine("Failed to acquire vJoy device number {0}.\n", player.vjoyDeviceID);
                return false;
            }
            else
                Console.WriteLine("Acquired: vJoy device number {0}.\n", player.vjoyDeviceID);

            Console.WriteLine("\npress enter to stat feeding");
            return true;
        }


    }

}