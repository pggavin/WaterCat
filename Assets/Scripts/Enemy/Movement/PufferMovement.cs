using System.Collections;
using UnityEngine;

public class PufferMovement : TurretMovement
{
    [SerializeField]
    private float _moveAmount = 4.0f;
    // How much it'll move
    private float _moveAmountPractical = 0.0f;
    // How much it's currently moving

    protected override void OnEnable()
    {
        StartCoroutine(StartMovement());
        base.OnEnable();
    }

    private void LateUpdate()
    {
        var newPos = new Vector2(SineWave(_moveAmountPractical), transform.position.y);
        transform.position = Vector2.Lerp(transform.position, newPos, Time.deltaTime * 3);
        // We lerp towards the desired position so the movement looks a little more natural
    }

    private IEnumerator StartMovement()
    {
        yield return new WaitForSeconds(_lerpDuration);
        // Turret takes 1s to get into place
        _moveAmountPractical = _moveAmount;
    }

    private float SineWave(float amount) => Mathf.Sin(Time.timeSinceLevelLoad) * amount;
    // Generates sine wave using timesincelevelload, amount optional
}
