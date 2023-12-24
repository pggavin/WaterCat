using BulletScripts;
using System.Collections;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class BasicEnemy : PoolAccessor, IOnDeath, IOnFire
{
    private SpriteRenderer _sprite;

    [SerializeField]
    private float _initialDelay = 1.0f;
    [SerializeField]
    private float _burstDelay = 2.0f;
    [SerializeField]
    private float _shotDelay = 0.5f;
    [SerializeField]
    private uint _burstAmount = 2;


    void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(FireBurst), _initialDelay, _burstDelay);
    }

    private void FireBurst()
    {
        // Enemies shoot in bursts
        for (int i = 0; i < _burstAmount; i++)
        {
            var waitForDelay = new WaitForSeconds(i * _shotDelay);
            // Don't fire if this object is disabled
            if (gameObject.activeSelf)
                StartCoroutine(Fire(waitForDelay));
        }
    }

    private IEnumerator Fire(WaitForSeconds waitForDelay)
    {
        yield return waitForDelay;
        Shoot();
    }

    protected virtual void Shoot()
    {
        DequeueBullet(DEFAULT_BULLET);
        // Basic enemies are only built to shoot one type of bullet
    }

    public void Died()
    {
        VisualEffects.Instance
            .DoScreenShake(DAMAGE_SHAKE_MAGNITUDE, DAMAGE_SHAKE_DURATION);
        // Basic enemies do screenshake when they die, very cool.
    }

    public void Shot(uint bulletID)
    {
        _sprite.flipX = !_sprite.flipX;
        // enemies flip their sprites when they shoot because it's funny
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        CancelInvoke();
    }
    // Stops invoke for bullets, stops coroutines to be safe


    const float DAMAGE_SHAKE_MAGNITUDE = 1.25f;
    const float DAMAGE_SHAKE_DURATION = 0.1f;
    const int DEFAULT_BULLET = 0;
}
