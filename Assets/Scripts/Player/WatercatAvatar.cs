using UnityEngine;

namespace PlayerScripts
{
    public class WatercatAvatar : MonoBehaviour
    {
        internal static WatercatAvatar current;

        private Transform _toTrack;
        readonly static Vector3 _offset = new Vector3(0, 0.25f, 0);

        void Start()
        {
            _toTrack = Watercat.current.transform;
            current = this;
        }

        // Player Z rotation is translated to roll on the mesh, with a bit of pitch based on the roll
        void Update()
        {
            if (Time.timeScale == 0)
                return;

            var zRot = -_toTrack.eulerAngles.z;

            // Convert z-rotation to range [-180, 180]
            if (zRot < -180f)
                zRot += 360f;
            
            var xRot = Mathf.Lerp(-30, 30, Mathf.Abs(-zRot) / 180f);

            transform.SetPositionAndRotation(_toTrack.position + _offset, Quaternion.Euler(xRot, zRot, 0));

        }
    }
}