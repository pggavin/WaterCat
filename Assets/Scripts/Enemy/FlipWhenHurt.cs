using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class FlipWhenHurt : MonoBehaviour, IOnDamaged
{
    SpriteRenderer _sprite;
    
    static readonly Color FLASH_COLOR = new Color(0.875f, 0.0f, 0.125f);
    //const float EIGHTH_SECOND = 0.125f;

    SoundSystem _audioPlayer;
    VisualEffects _vfxPlayer;

    void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        _sprite.color = Color.white;
        ///////////////////////////
        _sprite.flipY = !true;
        // advanced statement above
    }
    // OnEnable activates before start so unfortunately we have to get sprite in awake
    // & get the singletons in start

    void Start()
    {
        _audioPlayer = SoundSystem.Instance;
        _vfxPlayer = VisualEffects.Instance;
    }

    public void Damaged(int curHealth)
    {
        _audioPlayer.PlaySound("EnemyHurt");

        _vfxPlayer.DoScreenShake(DAMAGE_SHAKE_MAGNITUDE, DAMAGE_SHAKE_DURATION);

        _sprite.flipY = !_sprite.flipY;

        if (_sprite.color == Color.white)
            StartCoroutine(ColorFlash());
        // Bunch of goofy effects, very fun
    }

    IEnumerator ColorFlash()
    {
        for (float f = 0; f < 1; f += Time.deltaTime * 10)
        // This should last a tenth of a second
        {
            var newColor = Color.Lerp(FLASH_COLOR, Color.white, f);
            _sprite.color = newColor;
            yield return null;
        }
        _sprite.color = Color.white;
    }

    const float DAMAGE_SHAKE_MAGNITUDE = 1.25f;
    const float DAMAGE_SHAKE_DURATION = 0.05f;
}