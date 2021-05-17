//-----------------------------------------------------------------------
// <copyright file="ARPlaneController.cs" company="Jam3 Inc">
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
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Jam3.AR
{
    /// <summary>
    /// AR plane controller.
    /// </summary>
    /// <seealso cref="MonoBehaviour" />
    public class ARPlaneController : MonoBehaviour
    {
        public GameObject PlaneObject = null;
        public ARPlaneManager planeManager = default;

        // Runtiome variables
        private Transform m_OriginTransform = null;
        private bool planeSet = false;

        /// <summary>
        /// Ons enable.
        /// </summary>
        private void OnEnable()
        {
            if (!Application.isEditor)
            {
                if (PlaneObject != null)
                {
                    PlaneObject.transform.position = new Vector3(0, 1000, 0);
                    PlaneObject.SetActive(false);
                }

                if (planeManager != null) planeManager.planesChanged += OnPlanesUpdated;
            }
        }

        /// <summary>
        /// Ons disable.
        /// </summary>
        private void OnDisable()
        {
            if (!Application.isEditor)
                if (planeManager != null) planeManager.planesChanged -= OnPlanesUpdated;
        }

        /// <summary>
        /// Ons planes updated.
        /// </summary>
        /// <param name="args">The ARPlanesChangedEventArgs args.</param>
        private void OnPlanesUpdated(ARPlanesChangedEventArgs args)
        {
            if (!Application.isEditor)
            {
                foreach (ARPlane plane in args.added)
                {
                    if (plane.alignment == PlaneAlignment.HorizontalUp || plane.alignment == PlaneAlignment.HorizontalDown)
                    {
                        Vector3 center = plane.center;
                        if (center.y <= PlaneObject.transform.position.y)
                        {
                            PlaneObject.transform.position = center;
                            PlaneObject.SetActive(true);
                        }
                    }
                }
            }
        }
    }
}
