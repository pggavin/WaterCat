using UnityEngine;
using BulletScripts;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]

public class FrogBoss : PoolAccessor, IOnDamaged, IOnFire, IOnDeath
{
    // Warning this will use a lot of coroutines lol

    // Variables for hands, audio, visual effects, and level spawner
    SpriteRenderer _sprite;
    float          _verticalPosition = DEFAULT_VERTICAL_POS;
    GameObject     _handLeft;
    GameObject     _handRight;

    SoundSystem    _audioPlayer;
    VisualEffects  _vfxPlayer;
    LevelSpawner   _levelSpawner;

    Phase _phase = Phase.Default;

    Coroutine      _bossAttacks;
    
    private new void Start()
    {
        base.Start();

        // Coroutine to enter the level and initialize variables
        StartCoroutine(EnterLevel());
        _sprite = GetComponent<SpriteRenderer>();

        _handLeft = transform.GetChild(0).gameObject;
        _handRight = transform.GetChild(1).gameObject;

        _audioPlayer = SoundSystem.Instance;
        _vfxPlayer = VisualEffects.Instance;
        _levelSpawner = LevelSpawner.Instance;

        _bossAttacks = StartCoroutine(DefaultAttacks());
    }

    // Coroutine to lerp frog into the desired position
    IEnumerator EnterLevel()
    {
        for (float f = 0f; f < 1f; f += Time.deltaTime)
        {
            _verticalPosition = Mathf.Lerp( DEFAULT_VERTICAL_POS, DESIRED_VERTICAL_POS, f );
            // Lerp frog into desired position
            yield return null;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Calculate sine wave-based horizontal movement
        var horizontalPosition = SineWave(HORIZONTAL_MOVEMENT_MULT);

        transform.position = new Vector2(horizontalPosition, _verticalPosition);
    }

    // Determine and switch to the next phase
    void DeterminePhase(int curHealth)
    {
        if (curHealth % 30 != 0)
            return;

        StopCoroutine(_bossAttacks);

        _phase = (Phase)(((int)_phase + 1) % 3);

        if (_phase == Phase.Default)
        {
            DefaultPhase();
        }
        else if (_phase == Phase.Zoning)
        {
            ZoningPhase();
        }
        else
        {
            BlockingPhase();
        }
    }

    // Coroutine to lerp the hands of the FrogBoss to the desired position
    private IEnumerator MoveHands(Vector2 desiredPos)
    {
        Vector2 currentPos = _handRight.transform.localPosition;
        for (float f = 0f; f < 1f; f += Time.deltaTime)
        {
            _handRight.transform.localPosition = Vector2.Lerp( currentPos, desiredPos, f );
            _handLeft.transform.localPosition = _handRight.transform.localPosition * INVERTED_VECTOR;

            // Lerp frog into desired position
            yield return null;
        }
    }

    // Spawn a circular pattern of bullets around the FrogBoss
    private void SpinShot()
    {
        const int totalObjects = 360 / SPIN_ANGLE_INCREMENT;

        // Change the angle for each iteration based on how much we want to shoot
        for (int i = 0; i < totalObjects; i++)
        {
            var angle = i * SPIN_ANGLE_INCREMENT * Mathf.Deg2Rad;
            var spawnPosition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * SPIN_ATTACK_RADIUS;
            spawnPosition += transform.position;
            var spawnRotation = Quaternion.Euler(0, 0, -i * SPIN_ANGLE_INCREMENT);

            DequeueBullet(ACID_BULLET).transform.SetPositionAndRotation(spawnPosition, spawnRotation);
        }
    }

    // Initialize and start the Default phase coroutine
    private void DefaultPhase()
    {
        StartCoroutine(MoveHands(HANDS_DEFAULT));
        _bossAttacks = StartCoroutine(DefaultAttacks());
    }

    // Initialize and start the Zoning phase coroutine
    private void ZoningPhase()
    {
        _bossAttacks = StartCoroutine(ZoningAttacks());
    }

    // Initialize and start the Blocking phase coroutine
    private void BlockingPhase()
    {
        StartCoroutine(MoveHands(HANDS_COVERING));
        _bossAttacks = StartCoroutine(BlockingAttacks());
    }

    // Coroutine for the Default phase attack pattern
    IEnumerator DefaultAttacks()
    {
        int attackTypeIndex = 0;

        yield return new WaitForSeconds(1.25f);

        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            attackTypeIndex++;

            attackTypeIndex %= 10;

            DequeueBullet(GOOP_BULLET);

            if (attackTypeIndex == 9)
            {
                DequeueBullet(GOOP_BULLET_MULTI);
                _vfxPlayer.DoScreenShake(0.75f, 0.25f);
            }
        }
    }

    // Coroutine for the Zoning phase attack pattern
    IEnumerator ZoningAttacks()
    {
        int attackTodoIndex = 0;

        while (!false) // lol
        {
            attackTodoIndex++;

            if ((attackTodoIndex &= 1) == 0)
            {
                StartCoroutine(ShootDarts());
            }
            else
            {
                SpinShot();
                _vfxPlayer.DoScreenShake(0.75f, 0.25f);
            }
            yield return new WaitForSeconds(3.0f);

            _levelSpawner.ForceSpawn(ENEMY_BUG_OFFSET + attackTodoIndex);

            yield return new WaitForSeconds(3.0f);
        }
    }

    // Coroutine for the Blocking phase attack pattern
    IEnumerator BlockingAttacks()
    {
        bool spawnEnemyOnLeft = true;
        while (true)
        {
            var enemyToSpawn = ENEMY_FLY_OFFSET + (spawnEnemyOnLeft ? 0 : 1);

            _levelSpawner.ForceSpawn(enemyToSpawn);
            spawnEnemyOnLeft = !spawnEnemyOnLeft;
            yield return new WaitForSeconds(4.75f);
        }
    }

    // Coroutine to spawn darts as projectiles for the Zoning phase
    IEnumerator ShootDarts()
    {
        for (int i = 0; i < 6; i++)
        {
            yield return new WaitForSeconds(0.25f * i);
            DequeueBullet(DART_BULLET);
        }
    }

    // Helper method to calculate sine wave movement
    float SineWave(float amount)
        => Mathf.Sin(Time.timeSinceLevelLoad) * amount;
    // Generates sine wave using timesincelevelload, amount optional

    // Implementation of the IOnDamaged interface
    public void Damaged(int curHealth)
    {
        _audioPlayer.PlaySound("BossHurt");

        DeterminePhase(curHealth);

        _sprite.flipY = !_sprite.flipY;

        if (_sprite.color == Color.white)
            StartCoroutine(ColorFlash());
    }

    // Coroutine to make the sprite flash colors when the boss is damaged
    IEnumerator ColorFlash()
    {
        for (float f = 0; f < 1; f += Time.deltaTime * 8)
        // This should last an eighth of a second
        {
            var newColor = Color.Lerp(Color.red, Color.white, f);
            _sprite.color = newColor;
            yield return null;
        }
        _sprite.color = Color.white;
    }

    public void Shot(uint bulletID)
    {
        _sprite.flipX = !_sprite.flipX;
    }

    // Shake screen when dead!
    public void Died()
    {
        _vfxPlayer.DoScreenShake(0.5f, 0.75f);
    }

    public enum Phase
    {
        Default, // Shoots standard bullets, spawns frogs periodically
        Zoning, // Shoots darts, spawns bugs periodically
        Blocking,// Blocks itself, spawns flies periodically
    }

    const float HORIZONTAL_MOVEMENT_MULT = 3.5f;
    const float DEFAULT_VERTICAL_POS = 4.5f;
    const float DESIRED_VERTICAL_POS = 2.75f;

    readonly static Vector2 HANDS_DEFAULT = new Vector2(2.5f, -1);
    readonly static Vector2 HANDS_COVERING = new Vector2(0.75f, -0.8f);
    readonly static Vector2 INVERTED_VECTOR = new Vector2(-1.0f, 1.0f);

    const float SPIN_ATTACK_RADIUS = 0.5f;
    const int SPIN_ANGLE_INCREMENT = 60;

    const int ENEMY_FLY_OFFSET = 2;
    const int ENEMY_BUG_OFFSET = 4;

    const int GOOP_BULLET = 0;
    const int GOOP_BULLET_MULTI = 1;
    const int ACID_BULLET = 2;
    const int DART_BULLET = 3;
}
