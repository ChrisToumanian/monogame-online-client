using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using MonoGameTemplate.Engine.Networking;

namespace MonoGameTemplate.Engine
{
    class Chatbox
    {
        public List<string> messages = new List<string>();
        private int offset = 0;

        public Chatbox()
        {
        }

        public void Load()
        {
            Textbox chatbox0 = new Textbox(Assets.font, 20, 800, "");
            chatbox0.scale = 1.5f;
            Game.scene.textboxes.Add(chatbox0);

            Textbox chatbox1 = new Textbox(Assets.font, 20, 825, "");
            chatbox1.scale = 1.5f;
            Game.scene.textboxes.Add(chatbox1);

            Textbox chatbox2 = new Textbox(Assets.font, 20, 850, "");
            chatbox2.scale = 1.5f;
            Game.scene.textboxes.Add(chatbox2);

            Textbox chatbox3 = new Textbox(Assets.font, 20, 875, "");
            chatbox3.scale = 1.5f;
            Game.scene.textboxes.Add(chatbox3);

            Textbox chatbox4 = new Textbox(Assets.font, 20, 900, "");
            chatbox4.scale = 1.5f;
            Game.scene.textboxes.Add(chatbox4);

            Textbox chatbox5 = new Textbox(Assets.font, 20, 925, "");
            chatbox5.scale = 1.5f;
            Game.scene.textboxes.Add(chatbox5);

            Textbox chatbox6 = new Textbox(Assets.font, 20, 950, "");
            chatbox6.scale = 1.5f;
            Game.scene.textboxes.Add(chatbox6);

            Textbox chatbox7 = new Textbox(Assets.font, 20, 975, "");
            chatbox7.scale = 1.5f;
            Game.scene.textboxes.Add(chatbox7);

            Textbox chatbox8 = new Textbox(Assets.font, 20, 1000, "[Press Enter to chat]");
            chatbox8.scale = 1.5f;
            Game.scene.textboxes.Add(chatbox8);
        }

        public void Update()
        {
            int n = 7;
            for (int i = messages.Count - 1 - offset; i > -1; i--)
            {
                Game.scene.textboxes[n].text = messages[i];
                n--;
                if (n < 0) break;
            }
        }

        public void AddMessage(string message)
        {
            messages.Add(message);
            offset = 0;
            Update();
        }

        public void SendMessage(string message)
        {
            Client.Send(message);
        }

        public void ScrollUp()
        {
            if (offset < messages.Count - 5)
                offset += 1;
            Update();
        }

        public void ScrollDown()
        {
            if (offset != 0)
                offset -= 1;
            Update();
        }

        public void CaptureInput()
        {
            Game.scene.textboxes[8].text = Game.textInputBuffer;
            Update();
        }

        public void SendBuffer()
        {
            if (Game.textInputBuffer != "")
            {
                SendMessage(Game.textInputBuffer);
                Game.textInputBuffer = "";
            }
            Game.scene.textboxes[8].text = "[Press Enter to chat]";
        }
    }
}