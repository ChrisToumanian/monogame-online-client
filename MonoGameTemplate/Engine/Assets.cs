﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace MonoGameTemplate
{
    class Assets
    {
        // Scenes
        public static Scene introScene;

        // Textures
        public static Texture2D tiles;
        public static Texture2D sprites;
        public static Texture2D level01;
        public static Texture2D level01objects;
        public static Texture2D fontTexture;

        // Audio
        public static SoundEffect soundSword1;

        // Fonts
        public static Font font;

        // Load Game Assets
        public static void Load()
        {
            // Textures
            level01 = LoadImage(Program.game.GraphicsDevice, @"images/nexus.png");
            tiles = LoadImage(Program.game.GraphicsDevice, @"images/tiles.png");
            sprites = LoadImage(Program.game.GraphicsDevice, @"images/sprites.png");
            level01objects = LoadImage(Program.game.GraphicsDevice, @"images/nexus-objects.png");

            // Sounds
            Stream stream = File.OpenRead(@"audio/LTTP_Sword1.wav");
            soundSword1 = SoundEffect.FromStream(stream);

            // Fonts
            fontTexture = LoadImage(Program.game.GraphicsDevice, @"images/font.png");
            font = new Font(fontTexture, 256, 256, 16);

            // Intro Scene
            introScene = new Scenes.Map();
        }

        // Load Image
        public static Texture2D LoadImage(GraphicsDevice graphicsDevice, string filename)
        {
            FileStream setStream = File.Open(filename, FileMode.Open);
            Texture2D NewTexture = Texture2D.FromStream(Program.game.GraphicsDevice, setStream);
            setStream.Dispose();
            return NewTexture;
        }
    }
}
