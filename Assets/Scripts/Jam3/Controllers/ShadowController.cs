using UnityEngine;

namespace Jam3
{
    [RequireComponent(typeof(Projector))]
    public class ShadowController : MonoBehaviour
    {
        public Transform ScaleObjectTrasnform = null;

        private Projector cachedProjector = null;
        private float startNearClip = 0f;
        private float startFarClip = 0f;

        void Start()
        {
            cachedProjector = GetComponent<Projector>();
            startNearClip = cachedProjector.nearClipPlane;
            startFarClip = cachedProjector.farClipPlane;
        }

        void Update()
        {
            if (ScaleObjectTrasnform != null)
            {
                cachedProjector.nearClipPlane = startNearClip * ScaleObjectTrasnform.localScale.x;
                cachedProjector.farClipPlane = startFarClip * ScaleObjectTrasnform.localScale.x;
            }
        }
    }
}
