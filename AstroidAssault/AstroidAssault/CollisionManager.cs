using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Asteroid_Belt_Assault
{
    class CollisionManager
    {
        private AsteroidManager asteroidManager;
        private UpgradeAsteroidManager upgradeAsteroidManager;
        private PlayerManager playerManager;
        private EnemyManager enemyManager;
        private ExplosionManager explosionManager;
        private Vector2 offScreen = new Vector2(-500, -500);
        private Vector2 shotToAsteroidImpact = new Vector2(0, -20);
        private int enemyPointValue = 100;

        private Random rand = new Random(System.Environment.TickCount);

        public CollisionManager(
            AsteroidManager asteroidManager,
            UpgradeAsteroidManager upgradeAsteroidManager,
            PlayerManager playerManager,
            EnemyManager enemyManager,
            ExplosionManager explosionManager)
        {
            this.upgradeAsteroidManager = upgradeAsteroidManager;
            this.asteroidManager = asteroidManager;
            this.playerManager = playerManager;
            this.enemyManager = enemyManager;
            this.explosionManager = explosionManager;
        }

        private void checkShotToEnemyCollisions()
        {
            foreach (Sprite shot in playerManager.PlayerShotManager.Shots)
            {
                foreach (Enemy enemy in enemyManager.Enemies)
                {
                    if (shot.IsCircleColliding(
                        enemy.EnemySprite.Center,
                        enemy.EnemySprite.CollisionRadius))
                    {
                        shot.Location = offScreen;
                        enemy.Destroyed = true;
                        playerManager.PlayerScore += enemyPointValue;
                        explosionManager.AddExplosion(
                            enemy.EnemySprite.Center,
                            enemy.EnemySprite.Velocity / 10);
                    }

                }
            }
        }

        private void checkShotToAsteroidCollisions()
        {
            foreach (Sprite shot in playerManager.PlayerShotManager.Shots)
            {
                foreach (Sprite asteroid in asteroidManager.Asteroids)
                {
                    if (shot.IsCircleColliding(
                        asteroid.Center,
                        asteroid.CollisionRadius))
                    {
                        shot.Location = offScreen;
                        asteroid.Velocity += shotToAsteroidImpact;
                    }
                }

                foreach (Sprite asteroid in upgradeAsteroidManager.UpgradeAsteroids)
                {
                    if (shot.IsCircleColliding(
                        asteroid.Center,
                        asteroid.CollisionRadius))
                    { 
                        asteroidManager.AddAsteroid(shot.Location + new Vector2(-20, 0));
                        asteroidManager.Asteroids[asteroidManager.Asteroids.Count - 1].Velocity = asteroid.Velocity + new Vector2((float)rand.Next(-20, 20), (float)rand.Next(-20, 20));

                        asteroidManager.AddAsteroid(shot.Location + new Vector2(20, 15));
                        asteroidManager.Asteroids[asteroidManager.Asteroids.Count - 1].Velocity = asteroid.Velocity + new Vector2((float)rand.Next(-20, 20), (float)rand.Next(-20, 20)); ;

                        explosionManager.AddExplosion(shot.Location, asteroid.Velocity / 10);

                        shot.Location = offScreen;
                        asteroid.Velocity += shotToAsteroidImpact;
                        asteroid.Location = offScreen;
                    }
                }
            }
        }

        private void checkShotToPlayerCollisions()
        {
            foreach (Sprite shot in enemyManager.EnemyShotManager.Shots)
            {
                if (shot.IsCircleColliding(
                    playerManager.playerSprite.Center,
                    playerManager.playerSprite.CollisionRadius))
                {
                    shot.Location = offScreen;
                    playerManager.Destroyed = true;
                    explosionManager.AddExplosion(
                        playerManager.playerSprite.Center,
                        Vector2.Zero);
                }
            }
        }

        private void checkEnemyToPlayerCollisions()
        {
            foreach (Enemy enemy in enemyManager.Enemies)
            {
                if (enemy.EnemySprite.IsCircleColliding(
                    playerManager.playerSprite.Center,
                    playerManager.playerSprite.CollisionRadius))
                {
                    enemy.Destroyed = true;
                    explosionManager.AddExplosion(
                        enemy.EnemySprite.Center,
                        enemy.EnemySprite.Velocity / 10);

                    playerManager.Destroyed = true;

                    explosionManager.AddExplosion(
                        playerManager.playerSprite.Center,
                        Vector2.Zero);
                }
            }
        }

        private void checkAsteroidToPlayerCollisions()
        {
            foreach (Sprite asteroid in asteroidManager.Asteroids)
            {
                if (asteroid.IsCircleColliding(
                    playerManager.playerSprite.Center,
                    playerManager.playerSprite.CollisionRadius))
                {
                    explosionManager.AddExplosion(
                        asteroid.Center,
                        asteroid.Velocity / 10);

                    asteroid.Location = offScreen;

                    playerManager.Destroyed = true;
                    explosionManager.AddExplosion(
                        playerManager.playerSprite.Center,
                        Vector2.Zero);
                }
            }

            foreach (Sprite asteroid in upgradeAsteroidManager.UpgradeAsteroids)
            {
                if (asteroid.IsCircleColliding(
                    playerManager.playerSprite.Center,
                    playerManager.playerSprite.CollisionRadius))
                {
                    explosionManager.AddExplosion(
                        asteroid.Center,
                        asteroid.Velocity / 10);

                    asteroid.Location = offScreen;

                    playerManager.Destroyed = true;
                    explosionManager.AddExplosion(
                        playerManager.playerSprite.Center,
                        Vector2.Zero);
                }
            }
        }

        public void DoGravityBomb(Vector2 location)
        {
            foreach (Sprite asteroid in asteroidManager.Asteroids)
            {
                float dist = Vector2.Distance(location, asteroid.Center);

                Vector2 dir = location - asteroid.Center;
                dir.Normalize();

                dir *= Math.Min(600, 5000 * (1 / (dist/4)));

                asteroid.ImpulseVelocity = dir;
            }

            foreach (Enemy enemy in enemyManager.Enemies)
            {
                float dist = Vector2.Distance(location, EnemySprite.Center);

                Vector2 dir = location - asteroid.Center;
                dir.Normalize();

                dir *= Math.Min(600, 5000 * (1 / (dist / 4)));

                asteroid.ImpulseVelocity = dir;
            }
        }

        public void CheckCollisions()
        {
            MouseState ms = Mouse.GetState();
            DoGravityBomb(new Vector2((float)ms.X, (float)ms.Y));

            checkShotToEnemyCollisions();
            checkShotToAsteroidCollisions();
            if (!playerManager.Destroyed)
            {
                checkShotToPlayerCollisions();
                checkEnemyToPlayerCollisions();
                checkAsteroidToPlayerCollisions();
            }
        }

    }
}
