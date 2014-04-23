using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstroidAssault
{
    class AstroidManager
    {
        private int screenWidth = 800;
        private int screenHeight = 600;
        private int screenPadding = 10;

        private Rectangle initialFrame;
        private int asteroidFrames;
        private Texture2D texture;

        public List<Sprite> Asteroids = new List<Sprite>();
        private int minSpeed = 60;
        private int maxSpeed = 120;

        private Random rand = new Random();

        public void AddAsteroid()
        {
            Sprite newAsteroid = new Sprite(
                new Vector2(-500, -500),
                texture,
                initialFrame,
                Vector2.Zero);
            for (int x = 1; x < asteroidFrames; x++)
            {
                newAsteroid.AddFrame(new Rectangle(
                    initialFrame.X + (initialFrame.Width * x),
                    initialFrame.Y,
                    initialFrame.Width,
                    initialFrame.Height));
            }
            newAsteroid.Rotation =
                MathHelper.ToRadians((float)rand.Next(0, 360));
            newAsteroid.CollisionRadius = 15;
            Asteroids.Add(newAsteroid);
        }

        public void Clear()
        {
            Asteroids.Clear();
        }

        public AstroidManager(
            int asteroidCount,
            Texture2D texture,
            Rectangle initialFrame,
            int asteroidFrames,
            int screenWidth,
            int screenHeight)
        {
            this.texture = texture;
            this.initialFrame = initialFrame;
            this.asteroidFrames = asteroidFrames;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            for (int x = 0; x < asteroidCount; x++)
            {
                AddAsteroid();
            }
        }

        private Vector2 randomLocation()
        {
            Vector2 location = Vector2.Zero;
            bool locationOK = true;
            int tryCount = 0;

            do
            {
                locationOK = true;
                switch (rand.Next(0, 3))
                {
                    case 0:
                        location.X = -initialFrame.Width;
                        location.Y = rand.Next(0, screenHeight);
                        break;

                    case 1:
                        location.X = screenWidth;
                        location.Y = rand.Next(0, screenHeight);
                        break;

                    case 2:
                        location.X = rand.Next(0, screenWidth);
                        location.Y = -initialFrame.Height;
                        break;
                }
                foreach (Sprite asteroid in Asteroids)
                {
                    if (asteroid.IsBoxColliding(
                        new Rectangle(
                            (int)location.X,
                            (int)location.Y,
                            initialFrame.Width,
                            initialFrame.Height)))
                    {
                        locationOK = false;
                    }
                }
                tryCount++;
                if ((tryCount > 5) && locationOK == false)
                {
                    location = new Vector2(-500, -500);
                    locationOK = true;
                }
            } while (locationOK == false);

            return location;
        }

        private Vector2 randomVelocity()
        {
            Vector2 velocity = new Vector2(
                rand.Next(0, 101) - 50,
                rand.Next(0, 101) - 50);
            velocity.Normalize();
            velocity *= rand.Next(minSpeed, maxSpeed);
            return velocity;
        }

        private bool isOnScreen(Sprite asteroid)
        {
            if (asteroid.Destination.Intersects(
                new Rectangle(
                    -screenPadding,
                    -screenPadding,
                    screenWidth + screenPadding,
                    screenHeight + screenPadding)
                    )
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (Sprite asteroid in Asteroids)
            {
                asteroid.Update(gameTime);
                if (!isOnScreen(asteroid))
                {
                    asteroid.Location = randomLocation();
                    asteroid.Location = randomVelocity();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Sprite // page 110

    }
}
