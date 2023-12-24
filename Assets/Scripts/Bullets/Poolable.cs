using UnityEngine;

namespace BulletScripts
{
    // Added to objects made through bullets pooling so they add themselves back to their respective pools once they're disabled
    // Also, they will disable themselves once they're invisible since the camera is static
    using Pool = System.Collections.Generic.Queue<Poolable>;

    public class Poolable : MonoBehaviour
    {
        private Pool _returnTo;
        // pool to return to once disabled

        public void SetPool(Pool pool)
        {
            _returnTo = pool;
        }

        private void OnBecameInvisible()
        {
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _returnTo.Enqueue(this);
        }
    }
}