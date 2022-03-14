using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vJoyInterfaceWrap;
using System.Windows.Input;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Timers;


namespace WinFormsApp1
{
    public class HostControl
    {
        public const int MAX_PLAYER_NUM = 4;
        public const int WAIT_MSECOND = 15;
        public const int CHECK_AUDIENCE_SPAN_MS = 8000;

        public PlayerController[] players = new PlayerController[MAX_PLAYER_NUM];
        public AudienceSeat.AudienceData[] audienceList = new AudienceSeat.AudienceData[AudienceSeat.SEAT_NUM];
        public int currEntryCount = 0;
        public int audienceCount = 0;

        private MainFrom form;
        private System.Timers.Timer timer = new System.Timers.Timer(CHECK_AUDIENCE_SPAN_MS);

        public void Initial()
        {
            form = new MainFrom(this);
            form.Show();
            this.InitialTimer();

            this.mainLoop();
        }

        public void mainLoop()
        {
            while (true)
            {
                for (int i = 0; i < MAX_PLAYER_NUM; i++)
                {
                    if (players[i] == null)
                    {
                        form.setPlayerText(players[i],i);
                        continue;
                    }
                    else
                    {
                        int keyInput = getKeybordInput();
                        players[i].setButtonInput(keyInput);
                        VjoyControllerSetting.setVJoyInput(players[i]);
                        form.setPlayerText(players[i],i);
                    }
                }
                form.updateForm();

                System.Threading.Thread.Sleep(WAIT_MSECOND);
                System.Windows.Forms.Application.DoEvents();
            }
        }


        public bool createPlayer(uint playernum,string name, PlayerController.Platform plt)
        {
            if (name == null || name.Equals(String.Empty))
                name = "Player" + playernum;

            string uuid = "UUID-" + playernum + "-" + name;

            if (players[playernum - 1] == null)
            {
                PlayerController player = new PlayerController(playernum,name, plt,uuid);
                player.joystick = new vJoy();
                bool isCorrect = VjoyControllerSetting.setupVjoyController(player);
                if (isCorrect)
                {
                    this.players[playernum - 1] = player;
                    player.resetInput();
                    this.currEntryCount++;
                    MessageBox.Show("Success add Player_"+ playernum + " !!!");
                    return true;
                }
                else
                {
                    MessageBox.Show("Error : Miss add Player_" + playernum);
                }
            }
            else
            {
                MessageBox.Show("Already exist Player_" + playernum+" !");
            }
            return false;
        }

        public void InitialAudienceList()
        {
            for(int i = 0; i < AudienceSeat.SEAT_NUM; i++)
            {
                var ad = new AudienceSeat.AudienceData();
                this.audienceList[i] = ad;
            }
        }

        private int getKeybordInput()
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

        public int getEmptyPlayerNum()
        {
            for(int i = 0; i < MAX_PLAYER_NUM; i++)
            {
                if(this.players[i] == null)
                {
                    return i+1;
                }
            }
            return -1;
        }

        private void InitialTimer()
        {
            this.timer.Elapsed += (sender, e) =>
            {
                new ToastContentBuilder()
           .AddText("Meduse-Hostからの通知")
           .AddText("観客席の更新を行いました。")
           .Show();
            };
            this.timer.Start();
        }


    }
}
