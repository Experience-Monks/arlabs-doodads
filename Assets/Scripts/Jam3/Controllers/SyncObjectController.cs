using UnityEngine;

namespace Jam3
{
    [ExecuteInEditMode]
    public class SyncObjectController : MonoBehaviour
    {
        public GameObject ObjectToSync = null;
        public Vector3 Offset = Vector3.zero;

        void LateUpdate()
        {
            if (ObjectToSync != null)
            {
                gameObject.transform.position = ObjectToSync.transform.position + Offset;
            }
        }
    }
}
