using System;

using UnityEngine;

namespace Jam3.AR
{
    [Serializable]
    public class AROcclusionImageConfig : MonoBehaviour
    {
        public bool OcclusionEnabled = true;
        [Space]
        [Range(0, 1)] public float OcclusionTransparency = 1.0f;
        [Range(0, 1)] public float OcclusionOffset = 0.48f;
        [Range(0, 20)] public float OcclusionFadeVelocity = 4.0f;
        [Space]
        [Range(0, 1)] public float TransitionSize = 0.2f;
        [Range(0, 20)] public float MaximumOcclusionDistance = 20f;
        [Range(0, 3)] public float MaximumOcclusionDistanceTransition = 0.05f;
    }
}
