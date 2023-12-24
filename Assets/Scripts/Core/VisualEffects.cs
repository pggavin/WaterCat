using UnityEngine;
using PlayerScripts;
using UnityEngine.U2D;
using System.Collections;
using Ran = UnityEngine.Random;
using SF = UnityEngine.SerializeField;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(PixelPerfectCamera))]

public class VisualEffects : MonoSingleton<VisualEffects>
{
    ParticleSystem _explosionEffect;
    ParticleSystem _playerExplosionEffect;
    GameObject _lightEffect;

    [SF] Material _playerDeathMaterial;

    public static int _score = 0;
    // You get points for each time you activate screenshake because screenshake is awesome

    protected override void Initialize()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
            return;

        _explosionEffect = GetComponentInChildren<ParticleSystem>();
        _playerExplosionEffect = transform.GetChild(1).GetComponent<ParticleSystem>();
        // Get the two particle systems attached to this gameobject, we disconnect them later

        _lightEffect = GetComponentInChildren<Light>().gameObject;
        // Light applied to player fluctuates on a sine wave because it was fully assymetical before, was bugging me

        _explosionEffect.transform.parent = null;
        _playerExplosionEffect.transform.parent = null;
        // We have orphaned these Particle Systems
    }

    void LateUpdate()
    {
        var lightYaw = SineWave(AVATAR_MAX_YAW);
        // We change the yaw of the light based on a sine wave
        var lightRotationVector = new Vector3(_lightEffect.transform.eulerAngles.x, lightYaw, 0);

        _lightEffect.transform.eulerAngles = lightRotationVector;
    }

    // Function that calls Screenshake coroutine 
    internal void DoScreenShake(float magnitude, float duration)
    {
        StartCoroutine(ScreenShake(magnitude, duration));
        // You get points for making the screen shake lol
        _score++;
    }

    // Screenshake coroutine, awesome
    IEnumerator ScreenShake(float amount = 2.5f, float duration = 0.25f)
    {
        for (float timeLeft = duration; timeLeft > 0; timeLeft -= Time.fixedDeltaTime)
        {
            var shakeAmount = amount * timeLeft;

            transform.position += RandomVector3(shakeAmount);
            yield return null;
        }
        transform.position = DEFAULT_CAMERA_POS;
    }

    // Used for enemy gibbing
    public void DoParticleExplosion(Vector3 playAt)
    {
        _explosionEffect.gameObject.transform.position = playAt;
        _explosionEffect.Play();
    }

    public void DoFreezeFrames(float duration)
    {
        StartCoroutine(FreezeFrames(duration));
    }
    // Easy way to call freezeframes coroutine, player uses this when they're hit

    IEnumerator FreezeFrames(float duration)
    {
        Time.timeScale = 0.0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1.0f;
    }

    public void PlayerDeathZoom(Vector3 position)
    {
        StopAllCoroutines();
        StartCoroutine(ZoomIntoAnim(1.25f, position));
        StartCoroutine(PlayerExplodeAnim(1.25f, position));
    }

    IEnumerator ZoomIntoAnim(float zoomDuration, Vector3 zoomPosition)
    {
        var pixelPerfectCam = GetComponent<PixelPerfectCamera>();

        for (float f = 0; f <= 1; f += Time.deltaTime / zoomDuration)
        {
            var easeValue = EaseOutBounce(f);

            var lerpedPPU = Mathf.Lerp(PPU_DEFAULT, PPU_DESTINATION, easeValue);
            // We increase the pixels per unit to zoom, since we're using pixel perfect camera
            var lerpedPos = Vector3.Lerp(Vector3.zero, zoomPosition, easeValue);

            pixelPerfectCam.assetsPPU = (int)lerpedPPU;
            transform.position = lerpedPos + DEFAULT_CAMERA_POS;

            yield return null;
        }

        pixelPerfectCam.assetsPPU = PPU_DESTINATION;
        transform.position = zoomPosition + DEFAULT_CAMERA_POS;
        // We set the values into their desired positions at the end just to make sure
    }

    IEnumerator PlayerExplodeAnim(float waitTime, Vector2 deathPos)
    {
        yield return new WaitForSeconds(waitTime / 2);

        WatercatAvatar.current.GetComponent<MeshRenderer>().material = _playerDeathMaterial;
        GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
        // Camera background set to black

        _playerExplosionEffect.transform.position = deathPos;
        _playerExplosionEffect.Play();

        yield return new WaitForSeconds(waitTime / 2);

        SceneTransitions.Instance.DoSceneTransition("MainMenu");
    }

    // Used to generate values for screenshake
    static Vector3 RandomVector3(float amt)
    {
        return new Vector3(
        /*X*/ Ran.Range(-amt, amt),
        /*Y*/ Ran.Range(-amt, amt),
        /*Z*/ -10
        );
    }

    // Sine wave for player sprite
    private float SineWave(float amount)
        => Mathf.Sin(Time.timeSinceLevelLoad) * amount;

    // Used for camera ease in animation on player death
    // Formula from https://easings.net/#easeInBack
    // Takes in a percentage value between 0 and 1, and returns a transformed value
    public static float EaseOutBounce(float x)
    {
        const float n1 = BOUNCE_AMPLITUDE;
        const float d1 = BOUNCE_PERIOD;

        // Check which interval x falls in and compute the eased value using the Bounce easing function with an "out" easing mode
        if (x < 1 / d1)
        {
            return n1 * x * x;
        }
        else if (x < 2 / d1)
        {
            float easedValue = n1 * (x -= 1.5f / d1) * x + 0.75f;
            return easedValue;
        }
        else if (x < 2.5 / d1)
        {
            float easedValue = n1 * (x -= 2.25f / d1) * x + 0.9375f;
            return easedValue;
        }
        else
        {
            float easedValue = n1 * (x -= 2.625f / d1) * x + 0.984375f;
            return easedValue;
        }
    }

    const float BOUNCE_AMPLITUDE = 7.5625f;
    const float BOUNCE_PERIOD = 2.75f;

    const int PPU_DEFAULT = 32;
    const int PPU_DESTINATION = 136;

    const float AVATAR_MAX_YAW = 30.0f;

    static readonly Vector3 DEFAULT_CAMERA_POS = new Vector3(0, 0, -10);
}
