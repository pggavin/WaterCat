using UnityEngine;
using System.Collections;

namespace BulletScripts
{
    public class ManageChildBullets : MonoBehaviour
    {
        internal void CheckChildrenStatus()
        {
            StartCoroutine(ActiveCheck());
            // coroutine to prevent timing complications
        }

        private IEnumerator ActiveCheck()
        {
            yield return new WaitForEndOfFrame();
            // Waits for end of frame
            // some timing issues are present if both are disabled the same frame 
            if (GetComponentsInChildren<IShot>().GetLength(0) > 0)
                yield break;
            
            gameObject.SetActive(false);

            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(true);
            // Sets all child objects to true again for once the parent is reactivated
        }
    }
}