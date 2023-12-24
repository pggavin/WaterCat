using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    [SerializeField]
    private float _enemySpeed = 2.5f;


    [SerializeField]
    private Vector2 _moveDir = new Vector2(1, 0);

    void FixedUpdate()
    {
        transform.Translate(_enemySpeed * GenerateMovementMultiplier() * _moveDir);
        // Simply translates the enemy in its move direction
    }

    private void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }

    private float GenerateMovementMultiplier()
        => Mathf.Abs(Mathf.Sin(Time.timeSinceLevelLoad));
    // Enemy movement is an absolute value of a sine wave, because it looks cool
}
