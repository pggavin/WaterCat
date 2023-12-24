using BulletScripts;
using System.Collections;
using UnityEngine;

public class StarSummoner : PoolAccessor
{
    [SerializeField]
    private float _frequency = 0.75f;
    // How often we spawn stars

    private void OnEnable()
    {
        StartCoroutine(SpawnStarsAtInterval());
    }

    // Recursive coroutine to repeatedly spawn stars at a specified interval
    private IEnumerator SpawnStarsAtInterval()
    {
        yield return new WaitForSeconds(_frequency);
        SpawnStars();
        StartCoroutine(SpawnStarsAtInterval());
    }

    private void SpawnStars()
    {
        for (float x = X_MIN; x <= X_MAX; x += X_STEP)
        {
            float adjustedX = x + Random.Range(-VARIANCE, VARIANCE);

            // spawn bullet at adjusted position
            GameObject bullet = DequeueBullet(0);
            if (bullet != null)
            {
                bullet.transform.position = new Vector2(adjustedX, Y_POS);
            }
        }
    }

    const float X_MIN = -7.5f; // minimum x-coordinate
    const float X_MAX = 7.5f; // maximum x-coordinate
    const float X_STEP = 2.5f; // step size between x-coordinates
    const float Y_POS = 4.5f;
    const float VARIANCE = 0.5f; // random variance in position
}
