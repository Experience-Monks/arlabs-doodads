
//-----------------------------------------------------------------------
// <copyright file="ShadowController.cs" company="Jam3 Inc">
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

using UnityEngine;

namespace Jam3
{
    /// <summary>
    /// Shadow controller.
    /// </summary>
    /// <seealso cref="MonoBehaviour" />
    [RequireComponent(typeof(Projector))]
    public class ShadowController : MonoBehaviour
    {
        public Transform ScaleObjectTrasnform = null;

        // Runtime varilables
        private Projector cachedProjector = null;
        private float startNearClip = 0f;
        private float startFarClip = 0f;

        /// <summary>
        /// Start.
        /// </summary>
        private void Start()
        {
            cachedProjector = GetComponent<Projector>();
            startNearClip = cachedProjector.nearClipPlane;
            startFarClip = cachedProjector.farClipPlane;
        }

        /// <summary>
        /// Update.
        /// </summary>
        private void Update()
        {
            if (ScaleObjectTrasnform != null)
            {
                cachedProjector.nearClipPlane = startNearClip * ScaleObjectTrasnform.localScale.x;
                cachedProjector.farClipPlane = startFarClip * ScaleObjectTrasnform.localScale.x;
            }
        }
    }
}
