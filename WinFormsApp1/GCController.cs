
class GCController
{
    public enum ButtonBit
    {
        A = 0x01,
        B = 0x02,
        X = 0x04,
        Y = 0x08,
        Z = 0x10,
        START = 0x20,
        L = 0x40,
        R = 0x80,
        UP = 0x0100,
        DOWN = 0x0200,
        LEFT = 0x0400,
        RIGHT = 0x0800
    }

    int buttons;
    int analogStickX;
    int analogStickY;
    int CStickX;
    int CStickY;
    int PressureL;
    int PressureR;
    /*
        public const int A = 0x01;
        public const int B = 0x02;
        public const int X = 0x04;
        public const int Y = 0x08;
        public const int Z = 0x10;
        public const int START = 0x20;
        public const int L = 0x40;
        public const int R = 0x80;
        public const int UP = 0x0100;
        public const int DOWN = 0x0200;
        public const int LEFT = 0x0400;
        public const int RIGHT = 0x0800;
    */

    GCController()
    {
        this.buttons = 0;
        this.analogStickX = 0;
        this.analogStickY = 0;
        this.CStickX = 0;
        this.CStickY = 0;
        this.PressureL = 0;
        this.PressureR = 0;
    }

    public bool isPress(ButtonBit b)
    {
        return (this.buttons & (int)b) == (int)b;
    }

    public void getInput()
    {
        if (Console.KeyAvailable)
        {
            //if()
        }
        if (isPress(ButtonBit.A))
        {

        }
    }
}
