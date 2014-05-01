using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroid_Belt_Assault
{
    class UpgradeAsteroidManager
    {
        private int screenWidth = 800;
        private int screenHeight = 600;
        private int screenPadding = 10;

        private Rectangle initialFrame;
        private int UpgradeAsteroidFrames;
        private Texture2D texture;

        public List<Sprite> UpgradeAsteroids = new List<Sprite>();
        private int minSpeed = 60;
        private int maxSpeed = 120;

        private Random rand = new Random();

        public void AddUpgradeAsteroid()
        {
            Sprite newUpgradeAsteroid = new Sprite(
                new Vector2(-500, -500),
                texture,
                initialFrame,
                Vector2.Zero);
            for (int x = 1; x < UpgradeAsteroidFrames; x++)
            {
                newUpgradeAsteroid.AddFrame(new Rectangle(
                    initialFrame.X + (initialFrame.Width * x),
                    initialFrame.Y,
                    initialFrame.Width,
                    initialFrame.Height));
            }
            newUpgradeAsteroid.Rotation =
                MathHelper.ToRadians((float)rand.Next(0, 360));
            newUpgradeAsteroid.CollisionRadius = 15;
            UpgradeAsteroids.Add(newUpgradeAsteroid);
        }

        public void Clear()
        {
            UpgradeAsteroids.Clear();
        }

        public UpgradeAsteroidManager(
            int UpgradeasteroidCount,
            Texture2D texture,
            Rectangle initialFrame,
            int UpgradeasteroidFrames,
            int screenWidth,
            int screenHeight)
        {
            this.texture = texture;
            this.initialFrame = initialFrame;
            this.UpgradeAsteroidFrames = UpgradeasteroidFrames;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            for (int x = 0; x < UpgradeasteroidCount; x++)
            {
                AddUpgradeAsteroid();
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
                foreach (Sprite Upgradeasteroid in UpgradeAsteroids)
                {
                    if (Upgradeasteroid.IsBoxColliding(
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

        private bool isOnScreen(Sprite Upgradeasteroid)
        {
            if (Upgradeasteroid.Destination.Intersects(
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

        private void UpgradeBounceAsteroids(Sprite Upgradeasteroid1, Sprite Upgradeasteroid2)
        {
            {
                Vector2 cOfMass = (Upgradeasteroid1.Velocity +
                    Upgradeasteroid2.Velocity) / 2;

                Vector2 normal1 = Upgradeasteroid2.Center - Upgradeasteroid1.Center;
                normal1.Normalize();
                Vector2 normal2 = Upgradeasteroid1.Center - Upgradeasteroid2.Center;
                normal2.Normalize();

                Upgradeasteroid1.Velocity -= cOfMass;
                Upgradeasteroid1.Velocity =
                    Vector2.Reflect(Upgradeasteroid1.Velocity, normal1);
                Upgradeasteroid1.Velocity += cOfMass;

                Upgradeasteroid2.Velocity -= cOfMass;
                Upgradeasteroid2.Velocity =
                    Vector2.Reflect(Upgradeasteroid2.Velocity, normal2);

                Upgradeasteroid2.Velocity += cOfMass;
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (Sprite Upgradeasteroid in UpgradeAsteroids)
            {
                Upgradeasteroid.Update(gameTime);
                if (!isOnScreen(Upgradeasteroid))
                {
                    Upgradeasteroid.Location = randomLocation();
                    Upgradeasteroid.Velocity = randomVelocity();
                }
            }

            for (int x = 0; x < UpgradeAsteroids.Count; x++)
            {
                for (int y = x + 1; y < UpgradeAsteroids.Count; y++)
                {
                    if (UpgradeAsteroids[x].IsCircleColliding(
                        UpgradeAsteroids[y].Center, UpgradeAsteroids[y].CollisionRadius))
                    {
                        UpgradeBounceAsteroids(UpgradeAsteroids[x], UpgradeAsteroids[y]);
                    }
                }
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Sprite Upgradeasteroid in UpgradeAsteroids)
            {
                Upgradeasteroid.Draw(spriteBatch);
            }
        }


    }
}
