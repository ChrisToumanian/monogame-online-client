﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MonoGameTemplate
{
    class Scene
    {
        public string name;
        public GameObject player;
        public Rectangle bounds = new Rectangle(0, 0, 1920, 1080);
        public List<GameObject> gameObjects = new List<GameObject>();
        public List<Textbox> textboxes = new List<Textbox>();
        public List<Image> images = new List<Image>();
        public Camera camera;
        public TileMap map;
        public Color backgroundColor = Color.Black;
        public Image backgroundImage;
        public float parallax = 0;
        public bool updateGameObjectsOutsideOfView = false;
        public bool loaded = true;
        public byte[,] collisionMap;

        public Scene()
        {
        }

        public virtual void Load()
        {

        }

        public void Reload()
        {
            loaded = false;
            map = new TileMap(bounds.Width, bounds.Height, 16);
            gameObjects = new List<GameObject>();
            Load();
            loaded = true;
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public void UpdateGameObjects(GameTime gameTime)
        {
            if (loaded)
            {
                if (updateGameObjectsOutsideOfView)
                {
                    for (int i = 0; i < gameObjects.Count; i++)
                    {
                        gameObjects[i].Update();
                    }
                }
                else
                {
                    for (int i = 0; i < gameObjects.Count; i++)
                    {
                        if (gameObjects[i].enabled && Geometry.IsOver(gameObjects[i].transform.position, camera.bounds, 256))
                        {
                            gameObjects[i].Update();
                        }
                    }
                }
            }
        }

        public void UpdateParallax()
        {
            backgroundImage.transform.position.X = camera.bounds.X * parallax;
            backgroundImage.transform.position.Y = camera.bounds.Y * parallax;
        }

        public void AddGameObject(GameObject gameObject, int x, int y)
        {
            gameObject.transform.SetPosition(x, y);
            gameObject.scene = this;
            gameObjects.Add(gameObject);
        }

        public void AddGameObject(GameObject gameObject, int id, int x, int y)
        {
            gameObject.transform.SetPosition(x, y);
            gameObject.scene = this;
            gameObject.id = id;
            gameObjects.Add(gameObject);
        }

        public GameObject GetGameObject(int id)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (gameObjects[i].id == id)
                    return gameObjects[i];
            }
            return null;
        }

        public List<GameObject> GetGameObjects(Vector2 position)
        {
            List<GameObject> objects = new List<GameObject>();
            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (Geometry.IsOver(position, gameObjects[i].collider.GetBounds()))
                {
                    objects.Add(gameObjects[i]);
                }
            }
            return objects;
        }

        public List<GameObject> GetGameObjects(Rectangle rectangle)
        {
            List<GameObject> objects = new List<GameObject>();
            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (Geometry.IsOverlapping(rectangle, gameObjects[i].collider.GetBounds()))
                {
                    objects.Add(gameObjects[i]);
                }
            }
            return objects;
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (gameObjects[i] == gameObject)
                {
                    gameObjects.Remove(gameObjects[i]);
                }
            }
        }

        public void ClickGameObjects(Vector2 position)
        {
            List<GameObject> objects = GetGameObjects(position);
            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].Click();
            }
        }

        public virtual void ReceiveMessage(string message)
        {

        }
    }
}
