using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;

namespace MonoGameTemplate.Engine.Networking
{
    class Client
    {
        public static bool running = false;
        public static Action<string> callback;
        public static List<string> queue;
        public static Thread listenThread;

        private static TcpClient clientSocket = new TcpClient();
        private static NetworkStream serverStream = default(NetworkStream);

        public static bool Initialize(string address, int port, Action<string> callback)
        {
            Client.callback = callback;

            // Connect to server
            try
            {
                clientSocket.Connect(address, port);
                serverStream = clientSocket.GetStream();
                queue = new List<string>();

                // Send initial message
                byte[] outStream = Encoding.ASCII.GetBytes("");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();

                // Start listen thread
                listenThread = new Thread(Listen);
                listenThread.Start();

                running = true;

                return true;
            }
            catch (Exception e)
            {
                running = false;
            }

            running = false;
            return false;
        }

        public static void Listen()
        {
            string message = "";
            serverStream = clientSocket.GetStream();
            int bufferSize = 256;
            byte[] inStream;

            while (running)
            {
                // read messages
                inStream = new byte[bufferSize];
                serverStream.Read(inStream, 0, bufferSize);
                message = Encoding.ASCII.GetString(inStream, 0, bufferSize);
                string[] messages = message.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < messages.Length - 1; i++)
                {
                    callback.Invoke(messages[i]);
                }
            }
        }

        public static void Send(string message)
        {
            if (running)
            {
                byte[] outStream = Encoding.ASCII.GetBytes(message + "\n");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
            }
        }

        public static void AddToQueue(string message)
        {
            if (running)
            {
                queue.Add(message);
            }
        }

        public static void HandleQueue()
        {
            // manage queue
            if (running && queue.Count > 0)
            {
                // clean queue
                if (queue.Count > 100)
                {
                    for (int i = 100; i > 0; i--)
                    {
                        queue.RemoveAt(i);
                    }
                }

                // send messages in queue
                for (int i = 0; i < queue.Count; i++)
                {
                    Send(queue[i]);
                    queue.RemoveAt(i);
                }
            }
        }
    }
}