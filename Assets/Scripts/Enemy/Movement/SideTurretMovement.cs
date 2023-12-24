using System.Collections;
using UnityEngine;

public class SideTurretMovement : MonoBehaviour
{
    [SerializeField]
    private float _xStart;
    [SerializeField]
    private float _xDesired;
    [SerializeField]
    protected float _lerpDuration = 1.0f;

    virtual protected void OnEnable()
    {
        var startVector = new Vector2(_xStart, transform.position.y);
        var desiredVector = new Vector2(_xDesired, transform.position.y);
        // Cached vectors based on current and desired positions, enemy only moves on X axis

        StartCoroutine(MoveIntoPosition(startVector, desiredVector));
    }

    private IEnumerator MoveIntoPosition(Vector2 startVector, Vector2 desiredVector)
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
}
