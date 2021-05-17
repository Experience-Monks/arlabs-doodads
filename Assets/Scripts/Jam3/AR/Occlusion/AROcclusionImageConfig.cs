//-----------------------------------------------------------------------
// <copyright file="AROcclusionImageConfig.cs" company="Jam3 Inc">
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

using System;

using UnityEngine;

namespace Jam3.AR
{
    /// <summary>
    /// A r occlusion image config.
    /// </summary>
    /// <seealso cref="MonoBehaviour" />
    [Serializable]
    public class AROcclusionImageConfig : MonoBehaviour
    {
        /// <summary>
        /// The occlusion enabled.
        /// </summary>
        public bool OcclusionEnabled = true;
        [Space]

        [Range(0, 1)]
        /// <summary>
        /// The occlusion transparency.
        /// </summary>
        public float OcclusionTransparency = 1.0f;

        [Range(0, 1)]
        /// <summary>
        /// The occlusion offset.
        /// </summary>
        public float OcclusionOffset = 0.48f;

        [Range(0, 20)]
        /// <summary>
        /// The occlusion fade velocity.
        /// </summary>
        public float OcclusionFadeVelocity = 4.0f;

        [Space]
        [Range(0, 1)]
        /// <summary>
        /// The transition size.
        /// </summary>
        public float TransitionSize = 0.2f;

        [Range(0, 20)]
        /// <summary>
        /// The maximum occlusion distance.
        /// </summary>
        public float MaximumOcclusionDistance = 20f;

        [Range(0, 3)]
        /// <summary>
        /// The maximum occlusion distance transition.
        /// </summary>
        public float MaximumOcclusionDistanceTransition = 0.05f;
    }
}
