using System.Collections.Generic;
using UnityEngine;

namespace BulletScripts
{
    using Pool = Queue<Poolable>;

    public class PoolAccessor : MonoBehaviour
    {
        [SerializeField]
        private List<string> _bulletIDs = new List<string>();

        private Pool[] _pools;

        private IOnFire _onFire;

        protected void Start()
        {
            _onFire = GetComponent<IOnFire>();

            _pools = FetchPools();
        }
        // In case you are using Start in a child class, please call base.Start();

        private Pool[] FetchPools()
        {
            var tempPools = new List<Pool>();

            _bulletIDs.ForEach(i =>
                tempPools.Add(BulletPooling.GetPoolByName(i)
            ));
            // Adds pools from Pooling script by ID

            return tempPools.ToArray();
        }

        // Release a new bullet from the queue with the id specified
        internal GameObject DequeueBullet(uint id)
        {
            if (_pools[id].Count <= 0)
                return null;
            // Pool's empty

            _onFire.Shot(id);

            var bullet = _pools[id].Dequeue().gameObject;

            bullet.transform.SetPositionAndRotation(transform.position, transform.rotation);
            bullet.SetActive(true);

            return bullet;
        }
        // this returns a gameobject so scripts can manipulate their position, rotation, etc. as desired
    }
}