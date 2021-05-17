//-----------------------------------------------------------------------
// <copyright file="CameraBillboard.cs" company="Jam3 Inc">
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

namespace Jam3.Effects
{
    /// <summary>
    /// Camera billboard.
    /// </summary>
    /// <seealso cref="MonoBehaviour" />
    public class CameraBillboard : MonoBehaviour
    {
        [Header("Config")]
        public bool IsCilindrical = true;

        [Header("Camera")]
        public Camera MainCamera = null;

        /// <summary>
        /// Awake.
        /// </summary>
        private void Awake()
        {
            if (MainCamera == null)
                MainCamera = Camera.main;
        }

        /// <summary>
        /// Lates update.
        /// </summary>
        private void LateUpdate()
        {
            if (MainCamera != null)
            {
                var target = MainCamera.transform.position;

                if (IsCilindrical)
                    target.y = transform.position.y;

                transform.LookAt(target);
            }
        }
    }
}
