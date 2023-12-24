using PlayerScripts;
using System.Collections;
using UnityEngine;

public class Boat : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(WaitForSpawn());
        // To prevent timing issues
    }

    private IEnumerator WaitForSpawn()
    {
        yield return new WaitForEndOfFrame();

        var yPos = Watercat.current.transform.position.y;
        transform.position = new Vector2(transform.position.x, yPos);
    }
    // Boat enemy matches the player's Y position on spawning
}
