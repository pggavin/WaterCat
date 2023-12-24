using BulletScripts;
using UnityEngine;

public class BulletReflector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<BasicBullet>(out BasicBullet bullet))
            return;
        // Returns if the collision has no bullet component

        if (!bullet._isEnemyBullet)
            bullet.StuntShot();
        // We call stuntshot which swaps its team and flips it
    }
}
