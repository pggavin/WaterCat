using Unity.Burst.CompilerServices;
using UnityEngine;
using static UnityEngine.Time;

namespace BulletScripts
{
    public class BasicBullet : MonoBehaviour, IShot
    {
        [Header("Bullet Properties")]
        public int _damage = 1;
        public float _speed = 10f;
        public float _acceleration = 0f;
        public float _turnSpeed = 0f;

        [Header("Angle Properties")]
        public float _zOffset = 0f;
        public float _variance = 0f;

        [Header("Additional Properties")]
        public bool _isChild = false;
        public bool _isEnemyBullet = true;

        private float _zOrigin;
        private float _originalSpeed;
        private bool _originallyEnemyBullet;


        private void Awake()
        {
            _zOrigin = transform.localEulerAngles.z;
            _originalSpeed = _speed;
            _originallyEnemyBullet = _isEnemyBullet;
        }

        // Caches original speed and angle, so they can be reset after each use.
        protected virtual void OnEnable()
        {
            var zAngle = _zOffset + Random.Range(-_variance, _variance);
            transform.eulerAngles += new Vector3(0, 0, zAngle);
            _isEnemyBullet = _originallyEnemyBullet;
        }

        // Adds offset and variance to bullet's angle
        private void OnDisable()
        {
            transform.localEulerAngles = new Vector3(0, 0, _zOrigin);
            _speed = _originalSpeed;
            // Since speed/angle can vary after use, it resets them both back to the default values.
            if (_isChild)
                CallChildManager();
        }

        protected virtual void FixedUpdate()
        {
            if (_turnSpeed != 0)
                transform.eulerAngles += new Vector3(0, 0, _turnSpeed * fixedDeltaTime);

            if (_acceleration != 0)
                _speed += _acceleration * fixedDeltaTime;

            transform.position -= _speed * fixedDeltaTime * transform.up;
        }
        // Comment this later lol

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //Debug.Log("collided with " + collision.gameObject);

            if (!collision.TryGetComponent<Health>(out Health health))
                return;

            if (_isEnemyBullet != health.GetIsEnemy())
                health.NegateHealth(_damage);
        }
        // Because of how the layers work, bullets will never collide with something that doesn't have Health

        private void OnBecameInvisible()
        {
            gameObject.SetActive(false);
        }
        // Bullets get disabled once offscreen


        private void CallChildManager()
        {
            var parent = transform.parent;
            transform.position = parent.position;

            if (!parent.gameObject.activeSelf)
                return;

            parent.GetComponent<ManageChildBullets>().CheckChildrenStatus();
        }

        // Bullets are simply disabled once they are offscreen. Different attack types may handle this differently.
        private void OnApplicationQuit()
        {
            _isChild = false;
        }
        // To prevent errors from quitting scene midgame in the editor. Unnecessary in build mode.

        public void StuntShot()
        {
            _speed = _originalSpeed;
            transform.eulerAngles += new Vector3(0, 0, 180);
            _isEnemyBullet = !_isEnemyBullet;
        }
        // For the shot interface
    }
}