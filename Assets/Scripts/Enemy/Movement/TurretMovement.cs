using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMovement : MonoBehaviour
{
    [SerializeField]
    private float _yStart;
    [SerializeField]
    private float _yDesired;
    [SerializeField]
    protected float _lerpDuration = 1.0f;

    virtual protected void OnEnable()
    {
        var startVector = new Vector2 (transform.position.x, _yStart);
        var desiredVector = new Vector2(transform.position.x, _yDesired);
        // Cached vectors based on current and desired positions, enemy only moves on Y axis

        StartCoroutine(MoveIntoPosition(startVector, desiredVector));
    }

    private IEnumerator MoveIntoPosition(Vector2 startVector, Vector2 desiredVector )
    {
        // Simple lerp to desired pos
        for (float f = 0; f < 1; f += Time.deltaTime / _lerpDuration)
        {
            transform.position = Vector2.Lerp(startVector, desiredVector, f);
            yield return null;
        }
        // match desired pos at the end just to make sure
        transform.position = desiredVector;
    }

    private void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }
    // Turtle enemy moves from top to bottom, this is so it will disable once it passes the edge of the screen
}
