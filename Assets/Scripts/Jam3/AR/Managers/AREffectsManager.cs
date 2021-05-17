//-----------------------------------------------------------------------
// <copyright file="AREffectsManager.cs" company="Jam3 Inc">
//
// Copyright 2021 Jam3 Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------


using UnityEngine.XR.ARFoundation;

using Jam3.Util;

namespace Jam3.AR
{
    /// <summary>
    /// AR effects manager.
    /// </summary>
    /// <seealso cref="Singleton<AREffectsManager>" />
    public class AREffectsManager : Singleton<AREffectsManager>
    {
        private AROcclusionManager occlusionManager;
        private AROcclusionImageEffect occlusionEffect;

        // Runtiome variables
        private bool isOcclusionSupported = true;
        private bool capabilitiesSet = false;
        private (bool isRequested, bool state) pendingToggleRequest = (false, false);

        /// <summary>
        /// Ons enable.
        /// </summary>
        private void OnEnable()
        {
            occlusionManager = FindObjectOfType<AROcclusionManager>();
            occlusionEffect = FindObjectOfType<AROcclusionImageEffect>();
        }

        /// <summary>
        /// Sets occlusion enabled.
        /// </summary>
        /// <param name="enabled">The enabled.</param>
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

        /// <summary>
        /// Toggles occlusion.
        /// </summary>
        public void ToggleOcclusion()
        {
            SetOcclusionEnabled(!occlusionEffect.IsEnabled);
        }


        /// <summary>
        /// Update.
        /// </summary>
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
