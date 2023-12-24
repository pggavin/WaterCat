using PlayerScripts;
using UnityEngine;

public class AimingEnemy : BasicEnemy
{
    private Transform _target;
    private new void Start()
    {
        _target = Watercat.current.transform;
        // We target the player 
        base.Start();
    }

    protected override void Shoot()
    {
        var bullet = DequeueBullet(AIMED_BULLET);

        if (bullet == null)
            return;
        // There's a chance max bullets are reached, if that happens we don't fire

        Vector2 vectorToTarget = transform.position - _target.position;
        var atan = Mathf.Atan2(vectorToTarget.x , vectorToTarget.y) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.AngleAxis(atan, Vector3.back);
        // This is just the same code the player uses to rotate towards the mouse basically
    }

    const int AIMED_BULLET = 0;
}