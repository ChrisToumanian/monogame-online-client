using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameTemplate.Engine;
using MonoGameTemplate.Engine.Networking;

namespace MonoGameTemplate
{
    class Game : Microsoft.Xna.Framework.Game
    {
        public static string playerUID = "8df0f74h3h";
        public static Scene scene;
        public static Entities.Player player;
        public static int frameCount = 0;

        public static Chatbox chatbox;
        public static List<string> messages = new List<string>();
        public static string textInputBuffer = "";

        private SpriteBatch batch;
        private SpriteSortMode mode = SpriteSortMode.FrontToBack;
        private GraphicsDeviceManager graphics;
        RenderTarget2D buffer;

        public static int width = 1920;
        public static int height = 1080;
        public static int gameWidth = 480;
        public static int gameHeight = 270;
        private static bool fullscreen = false;
        private static bool useDisplayResolution = false;
        private static bool mouseCursor = true;
        private static bool multisampling = false;

        // 1. Constructor
        public Game() : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = fullscreen;
            DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;

            if (useDisplayResolution)
            {
                width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }

            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.PreferMultiSampling = multisampling;
            graphics.HardwareModeSwitch = false;
            IsMouseVisible = mouseCursor;

            // Text input handler
            Window.TextInput += TextInputHandler;

            this.IsFixedTimeStep = true;
        }

        // Load
        protected override void LoadContent()
        {
            // Create chatbox instance
            chatbox = new Chatbox();

            // Load Images
            Assets.Load();

            // Create Player
            player = new Entities.Player();

            // Load First Scene
            scene = Assets.introScene;
            scene.Load();

            // Send initial messages
            Client.Send("/uid " + playerUID);
            Client.Send("/id");

            // Create Sprite Batch
            batch = new SpriteBatch(GraphicsDevice);
            buffer = new RenderTarget2D(GraphicsDevice, gameWidth, gameHeight);
        }

        // Update
        protected override void Update(GameTime gameTime)
        {
            HandleMessages();
            scene.UpdateGameObjects(gameTime);
            scene.Update(gameTime);
            Controller.Update();

            // Send updates to server
            frameCount++;
            if (frameCount > 2)
            {
                Client.AddToQueue("/move " + player.id.ToString() + " " + ((int)player.transform.position.X).ToString() + " " + ((int)player.transform.position.Y).ToString());
                Client.HandleQueue();
                frameCount = 0;
            }
        }

        // Handle Messages
        private void HandleMessages()
        {
            for (int i = 0; i < messages.Count; i++)
            {
                // Add to chat messages
                if (scene != null && messages[i][0] != '/')
                {
                    chatbox.AddMessage(messages[i]);
                }
                else if (player != null) // Handle command
                {
                    string[] cmd = messages[i].Split(' ');

                    if (cmd[0] == "/move" && cmd.Length == 4) // Move position
                    {
                        int id = Int32.Parse(cmd[1]);
                        GameObject gameObject = scene.GetGameObject(id);

                        if (id != player.id)
                        {
                            if (gameObject != null)
                            {
                                gameObject.transform.SetPosition(Int32.Parse(cmd[2]), Int32.Parse(cmd[3]));
                            }
                            else
                            {
                                Entities.Player playerGameObject = new Entities.Player();
                                playerGameObject.controlsEnabled = false;
                                scene.AddGameObject(playerGameObject, id, Int32.Parse(cmd[2]), Int32.Parse(cmd[3]));
                            }
                        }
                    }
                    else if (cmd[0] == "/tp") // Teleport
                    {
                        if (Int32.Parse(cmd[1]) == player.id)
                        {
                            player.transform.SetPosition(Int32.Parse(cmd[2]), Int32.Parse(cmd[3]));
                        }
                    }
                    else if (cmd[0] == "/id") // Set ID
                    {
                        player.id = Int32.Parse(cmd[1]);
                        Client.AddToQueue("/coords");
                    }
                    else if (cmd[0] == "/spawn")
                    {
                        Entities.Player player = new Entities.Player();
                        player.controlsEnabled = false;
                        scene.AddGameObject(player, Int32.Parse(cmd[1]), Int32.Parse(cmd[2]), Int32.Parse(cmd[3]));
                    }
                    else if (cmd[0] == "/destroy")
                    {
                        int id = Int32.Parse(cmd[1]);
                        GameObject gameObject = scene.GetGameObject(id);

                        if (gameObject != null)
                        {
                            scene.RemoveGameObject(gameObject);
                        }
                    }
                }

                // Remove message from queue
                messages.RemoveAt(i);
                i--;
            }
        }

        // Draw
        protected override void Draw(GameTime gameTime)
        {
            if (buffer.Width != scene.camera.bounds.Width)
            {
                buffer = new RenderTarget2D(GraphicsDevice, scene.camera.bounds.Width, scene.camera.bounds.Height);
            }

            // Buffer
            GraphicsDevice.SetRenderTarget(buffer);
            GraphicsDevice.Clear(scene.backgroundColor);
            batch.Begin(mode, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

            // Draw Background
            if (scene.backgroundImage != null)
            {
                batch.Draw(scene.backgroundImage.sprite.texture,
                        new Vector2(scene.backgroundImage.transform.position.X - scene.camera.bounds.X, scene.backgroundImage.transform.position.Y - scene.camera.bounds.Y),
                        scene.backgroundImage.sprite.bounds,
                        scene.backgroundImage.sprite.color,
                        scene.backgroundImage.transform.rotation,
                        scene.backgroundImage.transform.origin,
                        scene.backgroundImage.transform.scale,
                        SpriteEffects.None,
                        scene.backgroundImage.sprite.depth);
            }

            // Draw Tiles
            /* if (scene.map != null)
            {
                Vector2 origin = new Vector2(0, 0);
                Rectangle view = scene.camera.bounds;
                int tileWidth = scene.map.tileWidth;
                
                int tileX = scene.camera.bounds.X / tileWidth;
                if (tileX < 0) tileX = 0;

                int tileY = scene.camera.bounds.Y / tileWidth;
                if (tileY < 0) tileY = 0;

                int columns = ((scene.camera.bounds.Width + scene.camera.bounds.X) / tileWidth) + 1;
                if (columns > scene.bounds.Width / tileWidth) columns = scene.bounds.Width / tileWidth;

                int rows = ((scene.camera.bounds.Height + scene.camera.bounds.Y) / tileWidth) + 1;
                if (rows > scene.bounds.Height / tileWidth) rows = scene.bounds.Height / tileWidth;

                for (int y = tileY; y < rows; y++)
                {
                    for (int x = tileX; x < columns; x++)
                    {
                        if (scene.map.tiles[x, y] != null)
                        {
                            batch.Draw(scene.map.tiles[x, y].sprite.texture,
                                new Vector2((x * tileWidth) - scene.camera.bounds.X, (y * tileWidth) - scene.camera.bounds.Y),
                                scene.map.tiles[x, y].sprite.bounds, scene.map.tiles[x, y].sprite.color,
                                0,
                                origin,
                                1,
                                scene.map.tiles[x, y].sprite.spriteEffect,
                                scene.map.tiles[x, y].sprite.depth);
                        }
                    }
                }
            }*/

            // Draw GameObjects
            for (int i = 0; i < scene.gameObjects.Count; i++)
            {
                if (scene.gameObjects[i].sprite != null && scene.gameObjects[i].enabled)
                {
                    float depth = scene.gameObjects[i].sprite.depth;
                    if (scene.gameObjects[i].sprite.useLevelDepth)
                    {
                        depth = scene.gameObjects[i].transform.position.Y * 0.0002f;
                    }

                    batch.Draw(scene.gameObjects[i].sprite.texture,
                        new Vector2((int)scene.gameObjects[i].transform.position.X - scene.camera.bounds.X, (int)scene.gameObjects[i].transform.position.Y - scene.camera.bounds.Y), 
                        scene.gameObjects[i].sprite.bounds, 
                        scene.gameObjects[i].sprite.color, 
                        scene.gameObjects[i].transform.rotation,
                        scene.gameObjects[i].transform.origin, 
                        scene.gameObjects[i].transform.scale,
                        scene.gameObjects[i].sprite.spriteEffect, 
                        depth);
                }
            }

            // Swap Buffer
            batch.End();
            GraphicsDevice.SetRenderTarget(null);
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

            batch.Draw(buffer, new Rectangle(0, 0, width, height), Color.White);


            // Draw Images
            for (int i = 0; i < scene.images.Count; i++)
            {
                if (scene.images[i].sprite != null)
                {
                    batch.Draw(scene.images[i].sprite.texture,
                        new Vector2(scene.images[i].transform.position.X, scene.images[i].transform.position.Y),
                        scene.images[i].sprite.bounds,
                        scene.images[i].sprite.color,
                        scene.images[i].transform.rotation,
                        scene.images[i].transform.origin,
                        scene.images[i].transform.scale,
                        SpriteEffects.None,
                        scene.images[i].sprite.depth);
                }
            }

            // Draw Textboxes
            for (int i = 0; i < scene.textboxes.Count; i++)
            {
                Textbox text = scene.textboxes[i];
                batch.DrawString(text.font.spriteFont, text.text, text.position, text.color, text.rotation, text.origin, text.scale, text.spriteEffect, text.depth);
            }

            batch.End();
        }

        // Unload
        protected override void UnloadContent()
        {
            batch.Dispose();
        }

        public static void Quit()
        {
            Client.running = false;
            Client.listenThread.Abort();
            Program.game.Exit();
        }

        // Receive Message
        public static void ReceiveMessage(string message)
        {
            messages.Add(message);
        }

        // Send Message
        public static void SendMessage(string message)
        {
            Client.Send(message);
        }

        // Text input handler
        private void TextInputHandler(object sender, TextInputEventArgs args)
        {
            var character = args.Character;
            textInputBuffer += character;
        }
    }
}