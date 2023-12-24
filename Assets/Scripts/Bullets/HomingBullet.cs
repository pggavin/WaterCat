using UnityEngine;
using PlayerScripts;
using SF = UnityEngine.SerializeField;

namespace BulletScripts
{
    public class HomingBullet : BasicBullet
    {
        Transform _target;
        
        [Header("Homing Bullet Properties")]
        [SF] float _turnRate      = 1.0f; 
        // controls how fast the bullet rotates towards the targetx
        
        [SF] float _speedVariance = 5.0f;
        // controls how fast the bullet rotates towards the target

        void Start()
        {
            _target = Watercat.current.transform;
        }

        protected override void OnEnable()
        {
            _speed += Random.Range(-_speedVariance, _speedVariance);
            base.OnEnable();
        }

        protected override void FixedUpdate()
        {
            // calculate direction towards target
            var direction = (transform.position - _target.position);

            // calculate target rotation
            var targetAngle = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg;

            // use Mathf.LerpAngle to smoothly interpolate between current and target rotation
            var newAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, _turnRate * Time.fixedDeltaTime);
            transform.eulerAngles = new Vector3(0, 0, newAngle);

            base.FixedUpdate();
        }
    }
}
