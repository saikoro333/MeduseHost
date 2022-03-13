using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vJoyInterfaceWrap;

namespace WinFormsApp1
{
    public class PlayerController
    {

        const int STICK_NUM = 2;
        const int VJOY_MAX_BUTTON = 20;

        //vjoy Device
        public VjdStat vJoyStatus;
        public vJoy joystick;
        public uint vjoyDeviceID = 1; //1 ~ 4

        //コントローラー情報
        public int playerNum = 0;
        public Platform platform;
        public enum Platform
        {
            Misc = -1,
            Wii = 0,
            GC = 1
        }

        //プレイヤー状態
        public string playerName;
        public PlayerStatus status;
        public enum PlayerStatus
        {
            NONE = 0,
            ONLINE = 1,
            BAN = 2,
            DISCONNECT = 3,
            OFFLINE = 4,
            BOT = 5
        }

        //ボタンの数などに関する変数
        public int nButtons, ContPovNumber, DiscPovNumber;

        //ボタン
        public int buttons = 0;

        //スティック
        public JoyStick[] sticks;

        public PlayerController(uint vid,string name, Platform plt)
        {
            this.vjoyDeviceID = vid;
            this.playerNum = (int)vid;
            this.playerName = name;
            this.platform = plt;
            this.status = PlayerStatus.ONLINE;
        }


        // Check which axes are supported
        public bool AxisX, AxisY, AxisZ, AxisRX, AxisRZ;
        public void setCheckAxisData(bool x,bool y,bool z,bool rx,bool rz)
        {
            this.AxisX = x; 
            this.AxisY = y;
            this.AxisZ = z;
            this.AxisRX = rx;
            this.AxisRZ = rz;
        }

        // Get the number of buttons and POV Hat switchessupported by this vJoy device
        public void setNumberOfButtons(int nb,int cp,int dp)
        {
            this.nButtons = nb;
            this.ContPovNumber = cp;
            this.DiscPovNumber = dp;
        }


        public uint count = 0;
        public long maxval = 0;

        public void preEnterLoop()
        {
            joystick.GetVJDAxisMax(vjoyDeviceID, HID_USAGES.HID_USAGE_X, ref maxval);
            joystick.GetVJDAxisMin(vjoyDeviceID, HID_USAGES.HID_USAGE_X, ref maxval);
            this.resetInput();
        }


        public void vJoyInput()
        {
            bool res;
            //res = joystick.SetAxis(X, id, HID_USAGES.HID_USAGE_X);
            //res = joystick.SetAxis(Y, id, HID_USAGES.HID_USAGE_Y);
            //res = joystick.SetAxis(Z, id, HID_USAGES.HID_USAGE_Z);
            //res = joystick.SetAxis(XR, id, HID_USAGES.HID_USAGE_RX);
            //res = joystick.SetAxis(ZR, id, HID_USAGES.HID_USAGE_RZ);

            // Press/Release Buttons
            //res = joystick.SetBtn(true, id, count / 50);
            //res = joystick.SetBtn(false, id, 1 + count / 50);
            //res = joystick.SetBtn(false, id, 5);


            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                res = joystick.SetBtn(true, vjoyDeviceID, 5);
            else
                res = joystick.SetBtn(false, vjoyDeviceID, 5);

            if (count % 100 == 0)
            {
                res = joystick.SetBtn(true, vjoyDeviceID, 1);
            }
            else
            {
                res = joystick.SetBtn(false, vjoyDeviceID, 1);
            }

            System.Threading.Thread.Sleep(20);
            count++;

            if (count > 640)
                count = 0;

            Application.DoEvents();
        }


        public void resetInput()
        {
            bool res;
            for (uint i = 0; i < this.nButtons; i++)
            {
                res = joystick.SetBtn(false, vjoyDeviceID, 1 + i);
            }

            for (uint i = 0; i < this.ContPovNumber; i++)
            {
                res = joystick.SetContPov(0, vjoyDeviceID, 1 + i);
            }
            for (uint i = 0; i < this.DiscPovNumber; i++)
            {
                res = joystick.SetDiscPov(0, vjoyDeviceID, 1 + i);
            }
        }

        public void setButtonInput(int key)
        {
            this.buttons = key;
        }
        public string InputToString()
        {
            String str = "";
            int key = this.buttons;
            switch (this.platform)
            {
                case Platform.Wii:
                    str = WiiController.HexToString((int)key);
                    break;
                    case Platform.GC:   
                    str = GCController.HexToString((int) key);
                    break;

                default:
                    str = "Misc...";
                    break;
            }
            return str;
        }

        public string showVerboseDetail()
        {

            string str = String.Format("PlayerNum : {0}\r\nPlayerName : {5}\r\n" +
                "VjoyDeviceID : {1}\r\n" +
                "ControllerPlatform : {2}\r\nMaxButtonNum : {3}\r\n" +
                "Input : {4}",playerNum,vjoyDeviceID,platform,nButtons,InputToString(),playerName);
            return str;
        }
        public string getStatus()
        {
            string str = "";
            switch (this.status)
            {
                case PlayerStatus.NONE:
                    str = "None";
                    break;
                case PlayerStatus.ONLINE:
                    str = "Online";
                    break;
                case PlayerStatus.BAN:
                    str = "BAN";
                    break;
                case PlayerStatus.DISCONNECT:
                    str = "Disconnect";
                    break;
                case PlayerStatus.OFFLINE:
                    str = "Offline";
                    break;
                case PlayerStatus.BOT:
                    str = "Bot";
                    break ;

                default:
                    str = "Error";
                    break;
            }
            return str;
        }

        public static Platform GetPlatformNum(string pltString)
        {
            switch (pltString)
            {
                case WiiController.PLATFORM:
                    return Platform.Wii;
                case GCController.PLATFORM:
                    return Platform.GC;

                default:
                    return Platform.Misc;
            }
        }

    }

    public class JoyStick
    {
        //stick [-1.0f - 1.0f] => [0 - 32767]
        public int axisX, axisY;
        public int maxX, maxY;
        public int minX, minY;

        public void setAxisRange(int x,int X,int y,int Y)
        {
            this.maxX = X;
            this.maxY = Y;
            this.minX = x;
            this.minY = y;
        }
    }
}
