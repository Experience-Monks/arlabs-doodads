//-----------------------------------------------------------------------
// <copyright file="PlacementManager.cs" company="Jam3 Inc">
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
using Jam3.Util;

namespace Jam3
{
    public class PlacementManager : Singleton<PlacementManager>
    {
        public Vector3 HitPosition { get => hitPosition; }
        public bool HasHitPoint { get; private set; }
        public bool IsBallPlaced { get; set; }
        public ARObject SelectedObject { get; private set; }
        public int InSceneObjectsCount { get; set; }

        public int MinObjectsInScene;
        public int MaxObjectsInScene;
        public float MaxPlacementDistance = 3f;

        public Layers SurfaceLayer = Layers.Surface;
        public Layers BackgroundLayer = Layers.BackgroundMesh;
        public GameObject TargetObject = null;

        // Runtime variables
        private int hitLayerMask = -1;
        private int hitLayerMaskSecundary = -1;

        private RaycastHit[] rayHits;
        private Vector3 outPosition = new Vector3(-1000f, -1000f, -1000f);
        private Vector3 hitPosition;

        /// <summary>
        /// Awake.
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            ShowTarget(false);
        }

        /// <summary>
        /// Start.
        /// </summary>
        private void Start()
        {
            HasHitPoint = false;
            rayHits = new RaycastHit[5];
            hitPosition = outPosition;
            hitLayerMask = 1 << (int)SurfaceLayer;
            hitLayerMaskSecundary = 1 << (int)BackgroundLayer;
        }

        /// <summary>
        /// Update.
        /// </summary>
        private void Update()
        {
            GetSurfaceHit();

            if (TargetObject != null)
                TargetObject.transform.position = hitPosition;
        }

        /// <summary>
        /// Shows target.
        /// </summary>
        /// <param name="show">The show.</param>
        public void ShowTarget(bool show)
        {
            if (TargetObject != null)
                TargetObject.SetActive(show);
        }

        /// <summary>
        /// Places object.
        /// </summary>
        public void PlaceObject()
        {
            if (SelectedObject != null)
            {
                if (SelectionManager.Instance.SelectedObject.IsPhysic)
                {
                    IsBallPlaced = true;
                }

                SelectionManager.Instance.UnselectObject();
                SelectedObject.Place();
            }

            Cancel();
        }

        /// <summary>
        /// Deletes object.
        /// </summary>
        public void DeleteObject()
        {
            if (SelectionManager.Instance.GetObject() != null)
            {
                if (SelectionManager.Instance.SelectedObject.IsPhysic)
                {
                    IsBallPlaced = false;
                }

                else if (InSceneObjectsCount > 0)
                {
                    InSceneObjectsCount--;
                }
            }

            Cancel();
            SelectionManager.Instance.DestroyObject();

            if (InSceneObjectsCount == 0 && !PopUpManager.Instance.IsShowing)
                PopUpManager.Instance.Show(0);
            else
                PopUpManager.Instance.CloseAll();
        }

        /// <summary>
        /// Cancel.
        /// </summary>
        public void Cancel()
        {
            if (SelectedObject != null)
                SelectedObject = null;
        }

        /// <summary>
        /// Sets selected object.
        /// </summary>
        /// <param name="arObject">The ar object.</param>
        public void SetSelectedObject(ARObject arObject)
        {
            if (SelectionManager.Instance.SelectedObject == null)
            {
                SelectedObject = arObject;
                SelectedObject.SetWorldPosition(hitPosition, -1f);
                SelectionManager.Instance.SelectObject(SelectedObject, false);
            }
        }

        /// <summary>
        /// Gets surface hit.
        /// </summary>
        private void GetSurfaceHit()
        {
            // Clear cache and flag
            HasHitPoint = false;
            hitPosition = outPosition;

            // Camera ray at the middle of the screen
            var cameraRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            // Camera pointing downwards
            if (cameraRay.direction.y < 0)
            {
                // Get nearest hit
                var nearestHitPoint = GetNearestHit(cameraRay, hitLayerMask);

                // Check if hit exist and if is in range
                if (nearestHitPoint.HasValue)
                {
                    var coplanarHitPoint = new Vector3(nearestHitPoint.Value.x, cameraRay.origin.y, nearestHitPoint.Value.z);
                    var coplanarDistanceToHitPoint = (coplanarHitPoint - cameraRay.origin).magnitude;
                    if (coplanarDistanceToHitPoint <= MaxPlacementDistance)
                    {
                        hitPosition = nearestHitPoint.Value;
                        HasHitPoint = true;
                    }
                }
            }

            // In case of camera pointing upwards, hitpoint out of range, or no hitpoint at all
            // will try to cast downwards at limit of the placement range, in the direction of the camera
            if (!HasHitPoint)
            {
                // Get camera horizontal direction
                var cameraPlanarDirection = cameraRay.direction;
                cameraPlanarDirection.y = 0;

                // Create a ray at limit position using previous direction
                var origin = cameraRay.origin + cameraPlanarDirection.normalized * MaxPlacementDistance;
                var direction = Vector3.down;
                var downwardsRay = new Ray(origin, direction);

                // Get nearest hit for new ray
                var nearestHitPoint = GetNearestHit(downwardsRay, hitLayerMask);

                // Check if hit exist and if is in range
                if (nearestHitPoint.HasValue)
                {
                    hitPosition = nearestHitPoint.Value;
                    HasHitPoint = true;
                }
            }
        }

        /// <summary>
        /// Gets the nearest hit for a given ray.
        /// </summary>
        /// <param name="ray">The ray.</param>
        /// <returns></returns>
        private Vector3? GetNearestHit(Ray ray, int layerMask)
        {
            var nearestHitPoint = default(Vector3?);
            var hits = Physics.RaycastNonAlloc(ray, rayHits, 100.0f, layerMask);
            if (hits > 0)
            {
                var nearstPointSqrDst = float.MaxValue;

                for (int i = 0; i < hits; i++)
                {
                    var sqrDst = Vector3.SqrMagnitude(rayHits[i].point - ray.origin);
                    if (sqrDst < nearstPointSqrDst)
                    {
                        nearestHitPoint = rayHits[i].point;
                        nearstPointSqrDst = sqrDst;
                    }
                }
            }
            return nearestHitPoint;
        }
    }
}
