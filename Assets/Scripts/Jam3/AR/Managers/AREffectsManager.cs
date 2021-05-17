using UnityEngine;
using UnityEngine.XR.ARFoundation;

using Jam3.Util;

namespace Jam3.AR
{
    public class AREffectsManager : Singleton<AREffectsManager>
    {
        private AROcclusionManager occlusionManager;
        private AROcclusionImageEffect occlusionEffect;

        private bool isOcclusionSupported = true;
        private bool capabilitiesSet = false;
        private (bool isRequested, bool state) pendingToggleRequest = (false, false);

        private void OnEnable()
        {
            occlusionManager = FindObjectOfType<AROcclusionManager>();
            occlusionEffect = FindObjectOfType<AROcclusionImageEffect>();
        }

        public void SetOcclusionEnabled(bool enabled)
        {
            //if we don't have capabilities set yet, store this request
            if (!capabilitiesSet)
            {
                pendingToggleRequest = (true, enabled);
            }
            else
            {
                // occlusionEffect.enabled = enabled && isOcclusionSupported;
                occlusionEffect.SetOcclusionEnabled(enabled && isOcclusionSupported);
                // occlusionManager.enabled = enabled && isOcclusionSupported;
            }
        }

        public void ToggleOcclusion()
        {
            SetOcclusionEnabled(!occlusionEffect.IsEnabled);
        }

        private void Update()
        {
            if (!capabilitiesSet && ARSession.state == ARSessionState.SessionTracking)
            {
                isOcclusionSupported = occlusionManager.descriptor.supportsEnvironmentDepthImage;
                capabilitiesSet = true;

                // if there was a pending toggle request, satisfy it
                if (pendingToggleRequest.isRequested)
                {
                    SetOcclusionEnabled(pendingToggleRequest.state);
                }
            }
        }
    }
}
