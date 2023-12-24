using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Object;

namespace BulletScripts
{
    using Pool = Queue<Poolable>;

    static public class BulletPooling
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void StartUp()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var bulletsInScene = _bulletsPerScene[scene.name];
            // Uses scene name as an index to access the bullets for it

            //ClearAllPools();
            _bulletsInPool = new Dictionary<string, Pool>();

            SoundSystem.Instance.NewScene(scene.buildIndex);

            LoadNewPool(bulletsInScene);
        }

        private static void LoadNewPool(BulletPoolData[] bulletsInScene)
        {
            var newPoolData = new Dictionary<string, Pool>();

            foreach (BulletPoolData bi in bulletsInScene)
            {
                var newPool = CreatePool(bi.key, bi.prewarmCount);
                newPoolData.Add(bi.key, newPool);
            }
            // Assign the new pool data to _bulletsInPool to update the pool data for the new scene
            _bulletsInPool = newPoolData;
        }

        private static Pool CreatePool(string key, uint prewarmCount)
        {
            // Load the bullet prefab from the "Bullets" subfolder of the Resources folder
            GameObject bulletPrefab = Resources.Load<GameObject>("Bullets/" + key);

            // Create a new BulletPool with prewarmCount objects
            var newPool = new Pool();

            for (int i = 0; i < prewarmCount; i++)
            {
                // Instantiate the bullet prefab and add it to the pool
                Poolable newBullet = Instantiate(bulletPrefab).AddComponent<Poolable>();
                // Adds Poolable component to bullet and sets its pool to this
                newBullet.SetPool(newPool);
                newPool.Enqueue(newBullet);
            }

            // Add the new pool to the new pool data
            return newPool;
        }

        public static Pool GetPoolByName(string id) => _bulletsInPool[id];

        private static Dictionary<string, Pool> _bulletsInPool = new();
        
        // Bullets used per scene
        // If this was an actual game I wouldn't hide it in a script I promise lol
        private static readonly Dictionary<string, BulletPoolData[]> _bulletsPerScene = new Dictionary<string, BulletPoolData[]>()
        {
            {
                "MainMenu",
                new BulletPoolData[]
                {
                    /* Nothing lol it's the menu */
                }
            },
            {
                "Scene1",
                new BulletPoolData[]
                {
                    new BulletPoolData("PlayerBullet", 25),
                    new BulletPoolData("QuillBullet", 10),
                    new BulletPoolData("PufferBullets", 25),
                    new BulletPoolData("CrabBullet"),
                }
            },
            {
                "Scene2",
                new BulletPoolData[]
                {
                    new BulletPoolData("PlayerBullet", 25),
                    new BulletPoolData("StarFishBullet", 35),
                    new BulletPoolData("ShellBullet", 10),
                    new BulletPoolData("AcidBullet", 10),
                    new BulletPoolData("DartBullet", 10),
                }
            },
            {
                "Scene3",
                new BulletPoolData[]
                {
                    new BulletPoolData("PlayerBullet", 25),
                    new BulletPoolData("GoopyBullets", 8),
                    new BulletPoolData("Goop1Bullet", 20),
                    new BulletPoolData("AcidBullet"),
                    new BulletPoolData("DartBullet", 10),
                }
            },
            {
                "WinScene",
                new BulletPoolData[]
                {
                    /* Nothing lol it's win */
                }
            },
            // Add more values as needed
        };

        [Serializable]
        public struct BulletPoolData
        {
            public string key;
            public uint prewarmCount;

            public BulletPoolData(string key, uint prewarmCount = 20)
            {
                this.key = key;
                this.prewarmCount = prewarmCount;
            }
        }
    }
}