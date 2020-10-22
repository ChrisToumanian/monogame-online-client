using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameTemplate
{
    class Program
    {
        public static Game game;
        public static string address = "mc.massivedamage.net";
        public static int port = 25566;
        public static bool online = false;

        static void Main(string[] args)
        {
            // Start game
            game = new Game();

            // Start client
            bool online = Engine.Networking.Client.Initialize(address, port, Game.ReceiveMessage);

            // Run game
            if (online)
            {
                game.Run();
            }
            else
            {
                game.Run();
                //Engine.Networking.Client.running = false;
            }
        }
    }
}