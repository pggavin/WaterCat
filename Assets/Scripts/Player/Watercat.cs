using UnityEngine;
using BulletScripts;
using System.Collections;
using System.Collections.Generic;

namespace PlayerScripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Watercat : PoolAccessor, IOnFire, IOnDamaged, IOnDeath
    {
        private Rigidbody2D _rb;
        private Camera _mc;
        private Vector2 _playerVelocity;
        //
        private bool _canParry = true;
        private int _delay = 0;

        const float _baseSpeed = 10f;
        const int _baseHealth = 20;

        internal static Watercat current;
        private GameObject _parryObject;

        private VisualEffects _vfxPlayer;
        private SoundSystem _audioPlayer;
        public PlayerHealthBar _healthBar;

        public bool PlayerDead { get; private set; }

        private void Awake()
        {
            current = this;
            // We don't use MonoSingleton here since this can't inherit from both that and PoolAccessor

            _parryObject = transform.GetChild(0).gameObject;
            _rb = GetComponent<Rigidbody2D>();
            _mc = Camera.main;
            _audioPlayer = SoundSystem.Instance;
            _vfxPlayer = VisualEffects.Instance;
            // Getting a lot of components...
        }
        private void FixedUpdate()
        {
            _playerVelocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            _rb.velocity = Vector2.ClampMagnitude(_playerVelocity, 1) * _baseSpeed;
            // ClampMagnitude is used so player isn't faster diagonally

            if (_delay < 0) _delay++;
            else if (Input.GetButton("Fire1"))
            {
                DequeueBullet(0);
                _delay = -5;
            }

            if (Input.GetButton("Jump") && _canParry)
            {
                Parry();
            }
            // Parry reflects the bullets nearby and looks cool
        }

        private void Update()
        {
            Vector2 vectorToTarget = Input.mousePosition - _mc.WorldToScreenPoint(transform.position);
            var atan = Mathf.Atan2(vectorToTarget.x, vectorToTarget.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(atan, Vector3.back);
            // Basically this is just used to make the player angle towards the mouse position
        }

        public void Shot(uint bulletID)
        {
            _audioPlayer.PlaySound("CatFired");
        }

        public void Died()
        {
            PlayerDead = true;
            _audioPlayer.PlaySound("CatDeath");
            _vfxPlayer.PlayerDeathZoom(transform.position);
        }

        private void Parry()
        {
            _audioPlayer.PlaySound("CatParry");
            StartCoroutine(ParryBullets());
        }
        
        // I'm sorry this is overcomplicated I just wanted it to be flashy and look cool
        private IEnumerator ParryBullets()
        {
            _parryObject.SetActive(true);
            // Parry Object is just a sprite for visual flair
            _canParry = false;
            var bulletsHit = new List<BasicBullet>();
            // All the bullets that will be reflected

            for (float f = 0.0f; f < 0.15f; f += Time.fixedDeltaTime)
            {
                // I know overlapcircle is expensive im sorry ;-;
                var hitObjects = Physics2D.OverlapCircleAll(transform.position, PARRY_RADIUS);

                foreach (Collider2D hit in hitObjects)
                {
                    if (!hit.TryGetComponent<BasicBullet>(out BasicBullet bullet) || !bullet._isEnemyBullet)
                        continue;

                    bullet._speed /= 3.0f;
                    // dividing speed of bullets, it looks cool

                    if (!bulletsHit.Contains(bullet))
                        bulletsHit.Add(bullet);
                }

                yield return new WaitForFixedUpdate();
            }

            bulletsHit.ForEach(b => b.StuntShot());
            _parryObject.SetActive(false);

            yield return new WaitForSeconds(2);

            _canParry = true;
        }

        public void Damaged(int curHealth)
        {
            _audioPlayer.PlaySound("CatHurt");
            _vfxPlayer.DoScreenShake(DAMAGE_SHAKE_MAGNITUDE, DAMAGE_SHAKE_DURATION);
            _vfxPlayer.DoFreezeFrames(FREEZE_FRAME_DURATION);
            _healthBar.UpdateHealth((float)curHealth / (float)_baseHealth);
            // When the player is damaged we do some fun effects and update the healthbar
        }

        const float DAMAGE_SHAKE_MAGNITUDE = 0.75f;
        const float DAMAGE_SHAKE_DURATION = 0.2f;
        const float FREEZE_FRAME_DURATION = 0.2f;
        const float PARRY_RADIUS = 1.25f;
        // Not sure if I should be putting consts at the bottom but I like it because I can separate the immutable values from the variables
    }
}