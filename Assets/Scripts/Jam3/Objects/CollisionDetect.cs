//-----------------------------------------------------------------------
// <copyright file="CollisionDetect.cs" company="Jam3 Inc">
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
    /// Used for knowing when to ARObject are colliding. 
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class CollisionDetect : MonoBehaviour
    {
        public ARObject Controller = null;

        // Runtime
        private bool collidingObject = false;
        private bool collidingSurface = false;

        private MeshCollider selfTriggerCollider = null;
        private MeshCollider selfMeshCollider = null;

        /// <summary>
        /// Awakes this instance.
        /// </summary>
        private void Awake()
        {
            var colliders = GetComponents<MeshCollider>();
            if (colliders.Length > 2)
                Debug.LogWarning("CollisionDetect:: You must have only 2 mesh colliders on this mesh and one must be convex");

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].convex)
                {
                    selfTriggerCollider = colliders[i];
                    selfTriggerCollider.isTrigger = true;
                }
                else
                {
                    selfMeshCollider = colliders[i];
                }
            }

            if (Controller == null)
                Controller = gameObject.GetComponentInParent<ARObject>();

            collidingSurface = false;
            collidingObject = false;
        }

        /// <summary>
        /// Called after regular update.
        /// </summary>
        private void FixedUpdate()
        {
            collidingSurface = false;
            collidingObject = false;
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        private void Update()
        {
            // if (Controller.IsCustomizable)
            // {
            //     if (collidingObject)
            //         Controller.Customization.ChangeColorAmount(1.0f);
            //     else
            //         Controller.Customization.ChangeColorAmount(0.0f);
            // }

            if (Controller.IsPhysic && Controller.Physics.AudioController != null)
            {
                Controller.Physics.AudioController.CanPlayAudio = collidingSurface;
            }
        }


        /// <summary>
        /// Called when [trigger stay].
        /// </summary>
        /// <param name="other">The other.</param>
        private void OnTriggerStay(Collider other)
        {
            if (Controller != null && Controller.Selected)
            {
                if (IsColliderObject(other))
                    collidingObject = true;
            }
        }

        /// <summary>
        /// Called when [collision enter].
        /// </summary>
        /// <param name="collisionInfo">The collision information.</param>
        private void OnCollisionEnter(Collision collisionInfo)
        {
            if (IsColliderSurface(collisionInfo.collider))
            {
                if (Controller.IsPhysic && Controller.Physics.AudioController != null)
                    Controller.Physics.AudioController.PlayHit();
            }
        }

        /// <summary>
        /// Called when [collision stay].
        /// </summary>
        /// <param name="collisionInfo">The collision information.</param>
        private void OnCollisionStay(Collision collisionInfo)
        {
            if (IsColliderSurface(collisionInfo.collider))
                collidingSurface = true;
        }

        /// <summary>
        /// Determines whether [other] is a candidate collidable object.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>
        ///   <c>true</c> if [other] is a candidate collidable object; otherwise, <c>false</c>.
        /// </returns>
        private bool IsColliderObject(Collider other)
        {
            bool isCollider = (other.gameObject.layer == (int)Layers.Draggable || other.gameObject.layer == (int)Layers.DraggableAction) &&
             other != selfTriggerCollider &&
             other != selfMeshCollider &&
             (other.GetType() == typeof(MeshCollider));

            return isCollider;
        }

        /// <summary>
        /// Determines whether [other] is a candidate collidable surface.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>
        ///   <c>true</c> if [other] is a candidate collidable surface; otherwise, <c>false</c>.
        /// </returns>
        private bool IsColliderSurface(Collider other)
        {
            bool isCollider = (other.gameObject.layer == (int)Layers.Surface ||
            other.gameObject.layer == (int)Layers.Draggable ||
            other.gameObject.layer == (int)Layers.BackgroundMesh) &&
             other != selfTriggerCollider &&
             other != selfMeshCollider &&
             (other.GetType() == typeof(MeshCollider));

            return isCollider;
        }
    }
}
