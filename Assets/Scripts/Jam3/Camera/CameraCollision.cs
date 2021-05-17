//-----------------------------------------------------------------------
// <copyright file="CameraCollision.cs" company="Jam3 Inc">
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
    /// Camera collision.
    /// </summary>
    /// <seealso cref="MonoBehaviour" />
    public class CameraCollision : MonoBehaviour
    {
        public bool IsOverMesh { get; private set; }
        public Layers MeshLayer = Layers.BackgroundMesh;

        /// <summary>
        /// Start.
        /// </summary>
        private void Start()
        {
            Reset();
        }

        /// <summary>
        /// Ons trigger stay.
        /// </summary>
        /// <param name="other">The other.</param>
        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == (int)MeshLayer)
                IsOverMesh = true;
            else
                IsOverMesh = false;
        }

        /// <summary>
        /// Ons trigger exit.
        /// </summary>
        /// <param name="other">The other.</param>
        private void OnTriggerExit(Collider other)
        {
            IsOverMesh = false;
        }

        /// <summary>
        /// Reset.
        /// </summary>
        public void Reset()
        {
            IsOverMesh = false;
        }
    }
}
