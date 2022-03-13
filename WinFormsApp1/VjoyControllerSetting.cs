using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    internal class VjoyControllerSetting
    {
        const bool showConsol = false;
        const bool showVerboseError = true;
        static public bool setupVjoyController(PlayerController player)
        {
            if (player.vjoyDeviceID <= 0 || player.vjoyDeviceID > 16)
            {
                if (showConsol) Console.WriteLine("Illegal device ID {0}\nExit!", player.vjoyDeviceID);
                return false;
            }

            if (!player.joystick.vJoyEnabled())
            {
                if (showConsol) Console.WriteLine("vJoy driver not enabled: Failed Getting vJoy attributes.\n");
                return false;
            }
            else
                if (showConsol) Console.WriteLine("Vendor: {0}\nProduct :{1}\nVersion Number:{2}\n", player.joystick.GetvJoyManufacturerString(), player.joystick.GetvJoyProductString(), player.joystick.GetvJoySerialNumberString());

            bool isCorrect = GetStatusDevice(player);
            if(!isCorrect)return false;
            isCorrect = TestMatchesDriver(player);
            if (!isCorrect && showVerboseError)
            {
                MessageBox.Show("Not Matches Driver!");
            }
            CheckContRanges(player);
            isCorrect = AquireTarget(player);
            return isCorrect;
        }
        static private bool GetStatusDevice(PlayerController player)
        {
            // Get the state of the requested device
            bool isShow = false;
            player.vJoyStatus = player.joystick.GetVJDStatus(player.vjoyDeviceID);
            switch (player.vJoyStatus)
            {
                case VjdStat.VJD_STAT_OWN:
                    if (showConsol) Console.WriteLine("vJoy Device {0} is already owned by this feeder\n", player.vjoyDeviceID);
                    if (isShow) MessageBox.Show("own");
                    break;
                case VjdStat.VJD_STAT_FREE:
                    if (showConsol) Console.WriteLine("vJoy Device {0} is free\n", player.vjoyDeviceID);
                    if (isShow) MessageBox.Show("free");
                    break;
                case VjdStat.VJD_STAT_BUSY:
                    if (showConsol) Console.WriteLine("vJoy Device {0} is already owned by another feeder\nCannot continue\n", player.vjoyDeviceID);
                    MessageBox.Show("busy");
                    return false;
                case VjdStat.VJD_STAT_MISS:
                    if (showConsol) Console.WriteLine("vJoy Device {0} is not installed or disabled\nCannot continue\n", player.vjoyDeviceID);
                    MessageBox.Show("miss");
                    return false;
                default:
                    if (showConsol) Console.WriteLine("vJoy Device {0} general error\nCannot continue\n", player.vjoyDeviceID);
                    MessageBox.Show("def");
                    return false;
            };
            return true;
        }

        static private bool TestMatchesDriver(PlayerController player)
        {
            UInt32 DllVer = 0, DrvVer = 0;
            bool match = player.joystick.DriverMatch(ref DllVer, ref DrvVer);
            if (match)
            {
                if (showConsol) Console.WriteLine("Version of Driver Matches DLL Version ({0:X})\n", DllVer);
                return true;
            }
            else
            {
                if (showConsol) Console.WriteLine("Version of Driver ({0:X}) does NOT match DLL Version ({1:X})\n", DrvVer, DllVer);
                return false;
            }
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

        static private bool AquireTarget(PlayerController player)
        {
            // Acquire the target
            if ((player.vJoyStatus == VjdStat.VJD_STAT_OWN) || ((player.vJoyStatus == VjdStat.VJD_STAT_FREE) && (!player.joystick.AcquireVJD(player.vjoyDeviceID))))
            {
                if (showConsol) Console.WriteLine("Failed to acquire vJoy device number {0}.\n", player.vjoyDeviceID);
                return false;
            }
            else
                if (showConsol) Console.WriteLine("Acquired: vJoy device number {0}.\n", player.vjoyDeviceID);

            if (showConsol) Console.WriteLine("\npress enter to stat feeding");
            return true;
        }

        static public void setVJoyInput(PlayerController player)
        {
            bool res;
            int key = player.buttons;
            for (int i = 0; i < 16; i++)
            {
                if (((key>>i) & 1) != 0)
                {
                    res = player.joystick.SetBtn(true, player.vjoyDeviceID, (uint)i + 1);
                }
                else
                {
                    res = player.joystick.SetBtn(false, player.vjoyDeviceID, (uint)i + 1);
                }
            }
        }

    }
}
