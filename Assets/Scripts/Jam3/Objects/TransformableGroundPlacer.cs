//-----------------------------------------------------------------------
// <copyright file="TransformableGroundPlacer.cs" company="Jam3 Inc">
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
    /// Transformable ground placer.
    /// </summary>
    /// <seealso cref="MonoBehaviour" />
    [RequireComponent(typeof(ARObject))]
    [RequireComponent(typeof(TransformableObject))]
    public class TransformableGroundPlacer : MonoBehaviour
    {
        #region Exposed fields

        [SerializeField]
        private Layers[] groundLayers = new[] { Layers.BackgroundMesh, Layers.Surface };

        #endregion Exposed fields

        #region Non Exposed fields

        private ARObject cachedArObjectComponent;
        private TransformableObject cachedTransformableObject;

        private int hitLayerMask = -1;
        private Ray downRay;
        private RaycastHit[] rayHits;

        private float initialWorldHeight;   // Local height 0
        private float initialHeightOffset;

        private const float RaycastYOffset = 100f; // Used to raycast above the object and detect surfaces over it

        #endregion Non Exposed fields

        #region Properties

        /// <summary>
        /// Gets or sets the layers.
        /// </summary>
        /// <value>
        /// The layers.
        /// </value>
        public Layers[] GroundLayers
        {
            get => groundLayers;
            set => groundLayers = value;
        }

        /// <summary>
        /// Gets the base.
        /// </summary>
        /// <value>
        /// The base.
        /// </value>
        public ARObject ARBase =>
            cachedArObjectComponent;

        /// <summary>
        /// Gets the transformable.
        /// </summary>
        /// <value>
        /// The transformable.
        /// </value>
        public TransformableObject Transformable =>
            cachedTransformableObject;

        #endregion Properties

        #region Custom Events

        #endregion Custom Events

        #region Events methods

        /// <summary>
        /// Awakes this instance.
        /// </summary>
        private void Awake()
        {
            // Get required components
            cachedArObjectComponent = GetComponent<ARObject>();
            cachedTransformableObject = GetComponent<TransformableObject>();

            // Create raycast auxiliary structs
            downRay = new Ray(Vector3.zero, Vector3.down);
            rayHits = new RaycastHit[5];

            // Set layermask
            if (groundLayers != null && groundLayers.Length > 0)
            {
                hitLayerMask = 0;
                foreach (var layer in GroundLayers)
                {
                    hitLayerMask |= 1 << (int)layer;
                }
            }

            // Cache initial values
            initialWorldHeight = ARBase.transform.position.y;
            initialHeightOffset = Transformable.TranslateMin.y;

            // Register events callbacks
            RegisterCallbacks();
        }

        /// <summary>
        /// Destroys this instance.
        /// </summary>
        private void OnDestroy()
        {
            UnregisterCallbacks();
        }


        /// <summary>
        /// Called when [position is set].
        /// </summary>
        /// <param name="position">The position.</param>
        private void OnPositionSet(Vector3 position)
        {
            UpdateHeightLimit();
        }

        /// <summary>
        /// Called when [rotation is set].
        /// </summary>
        /// <param name="rotation">The rotation.</param>
        private void OnRotationSet(Vector3 rotation)
        {
            UpdateHeightLimit();
        }

        /// <summary>
        /// Called when [scale is set].
        /// </summary>
        /// <param name="scale">The scale.</param>
        private void OnScaleSet(Vector3 scale)
        {
            UpdateHeightLimit();
        }

        #endregion Events methods

        #region Public Methods

        /// <summary>
        /// Updates the height limit.
        /// </summary>
        public void UpdateHeightLimit()
        {
            if (CheckGroundHeight(out float groundHeight))
            {
                // Set new limit
                Transformable.TranslateMin = new Vector3
                (
                    Transformable.TranslateMin.x,
                    groundHeight - initialWorldHeight + initialHeightOffset,
                    Transformable.TranslateMin.z
                );

                // Check if current position is under the limit
                var currentPosition = Transformable.ARBase.GetWorldPosition();
                if (currentPosition.y < groundHeight)
                {
                    currentPosition.y = groundHeight;
                    Transformable.ARBase.InternalSetWorldPosition(currentPosition, -1);
                }
            }
        }

        #endregion Public Methods

        #region Non Public Methods

        /// <summary>
        /// Registers the callbacks.
        /// </summary>
        private void RegisterCallbacks()
        {
            ARBase.PositionSetEvent += OnPositionSet;
            ARBase.RotationSetEvent += OnRotationSet;
            ARBase.ScaleSetEvent += OnScaleSet;
        }

        /// <summary>
        /// Registers the callbacks.
        /// </summary>
        private void UnregisterCallbacks()
        {
            ARBase.PositionSetEvent -= OnPositionSet;
            ARBase.RotationSetEvent -= OnRotationSet;
            ARBase.ScaleSetEvent -= OnScaleSet;
        }

        /// <summary>
        /// Checks the ground position.
        /// </summary>
        /// <param name="groundHeight">Height of the world.</param>
        /// <returns></returns>
        private bool CheckGroundHeight(out float groundHeight)
        {
            var hitGround = false;
            groundHeight = float.MinValue;

            // Get data from bounds
            var cornerGroundHeight = default(float);
            var center = Transformable.TargetBounds.WorldCenter;
            var extents = Transformable.TargetBounds.Extents;
            var rotation = Transformable.TargetBounds.Rotation;

            // Set each corner position
            var cornerA = center + rotation * new Vector3(extents.x, 0, extents.z);
            var cornerB = center + rotation * new Vector3(-extents.x, 0, extents.z);
            var cornerC = center + rotation * new Vector3(extents.x, 0, -extents.z);
            var cornerD = center + rotation * new Vector3(-extents.x, 0, -extents.z);

            // Check each corner
            if (GroundHeightAtPosition(cornerA, out cornerGroundHeight))
            {
                hitGround = true;
                if (cornerGroundHeight > groundHeight)
                    groundHeight = cornerGroundHeight;
            }
            if (GroundHeightAtPosition(cornerB, out cornerGroundHeight))
            {
                hitGround = true;
                if (cornerGroundHeight > groundHeight)
                    groundHeight = cornerGroundHeight;
            }
            if (GroundHeightAtPosition(cornerC, out cornerGroundHeight))
            {
                hitGround = true;
                if (cornerGroundHeight > groundHeight)
                    groundHeight = cornerGroundHeight;
            }
            if (GroundHeightAtPosition(cornerD, out cornerGroundHeight))
            {
                hitGround = true;
                if (cornerGroundHeight > groundHeight)
                    groundHeight = cornerGroundHeight;
            }

            return hitGround;
        }

        /// <summary>
        /// Grounds the height at position.
        /// </summary>
        /// <param name="worldPosition">The world position.</param>
        /// <returns></returns>
        private bool GroundHeightAtPosition(Vector3 worldPosition, out float groundHeight)
        {
            var hitGround = false;
            groundHeight = float.MinValue;

            downRay.origin = worldPosition + Vector3.up * RaycastYOffset;
            var hits = Physics.RaycastNonAlloc(downRay, rayHits, 200f, hitLayerMask, QueryTriggerInteraction.Ignore);
            if (hits > 0)
            {
                var highestHeight = float.MinValue;
                for (int i = 0; i < hits; i++)
                {
                    if (rayHits[i].point.y > highestHeight)
                        highestHeight = rayHits[i].point.y;
                }
                groundHeight = highestHeight;
                hitGround = true;
            }

            return hitGround;
        }

        #endregion Non Public Methods
    }
}
