using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGameTemplate.Scenes
{
    class Map : Scene
    {
        // Constructor
        public Map()
        {
            name = "Nexus";
            bounds = new Rectangle(0, 0, 2048, 2048);
            backgroundColor = Color.Black;
            camera = new Camera(this);
            camera.Zoom(0.50f);
        }

        // Load
        public override void Load()
        {
            // background image
            backgroundImage = new Image(Assets.level01);
            bounds.Width = backgroundImage.width;
            bounds.Height = backgroundImage.height;

            // extract collision data
            Color[] bgColors = new Color[bounds.Width * bounds.Height];
            backgroundImage.texture.GetData(bgColors);
            collisionMap = new byte[bounds.Width, bounds.Height];
            for (int x = 0; x < bounds.Width; x++)
            {
                for (int y = 0; y < bounds.Height; y++)
                {
                    if (bgColors[x + y * bounds.Width].R == 0)
                        collisionMap[x, y] = 0;
                    else
                        collisionMap[x, y] = 1;
                }
            }
            bgColors = null;

            // Create tile sprites
            map = new TileMap(bounds.Width, bounds.Height, 16);
            //map.AddTileType("grass01", Assets.tiles, 1, 1, 16, false, "81d1ad");
            //map.CreateMapFromTexture(Assets.level01);

            // Place Trees
            /*foreach (Vector2 position in map.GetPositionsByColor(Assets.level01, "496a1a"))
            {
                AddGameObject(new Scenery.SmallPineTree(), this, (int)position.X, (int)position.Y);
            }*/

            // Place Objects
            string[,] colors = map.GetHexColorsFromTexture(Assets.level01);
            colors = map.GetHexColorsFromTexture(Assets.level01objects);
            for (int x = 0; x < map.width; x++)
            {
                for (int y = 0; y < map.height; y++)
                {
                    string c = colors[x, y];
                    if (c == "496a1a")
                    {
                        Scenery.SmallPineTree tree = new Scenery.SmallPineTree();
                        AddGameObject(tree, x * map.tileWidth + 8, y * map.tileWidth + 8);
                    }
                }
            }

            // Add Player
            AddGameObject(Game.player, 128, 64);
            camera.parent = Game.player.transform;

            // Chatbox
            Game.chatbox.Load();

            loaded = true;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            // Exit game
            if (Controller.IsKeyDown(Keys.Escape)) // Exit game
            {
                Game.Quit();
            }

            // Chat Controls
            if (Controller.IsKeyDown(Keys.Enter)) // Exit game
            {
                if (Game.player.controlsEnabled)
                {
                    Game.player.controlsEnabled = false;
                    Game.player.animation.Stop();
                    Game.textInputBuffer = "";
                }
                else
                {
                    Game.chatbox.SendBuffer();
                    Game.player.controlsEnabled = true;
                }
            }
            else if (Controller.IsScrollingUp()) // Camera Zoom In
            {
                Game.chatbox.ScrollUp();
            }
            else if (Controller.IsScrollingDown()) // Camera Zoom Out
            {
                Game.chatbox.ScrollDown();
            }

            // Chat entry mode
            if (!Game.player.controlsEnabled)
            {
                Game.chatbox.CaptureInput();
            }

            // Camera Follow Player
            camera.Update();
        }
    }
}