//-----------------------------------------------------------------------
// <copyright file="Snapper.cs" company="Jam3 Inc">
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
    /// Snapper.
    /// </summary>
    /// <seealso cref="MonoBehaviour" />
    public class Snapper : MonoBehaviour
    {
        // Runtime
        private bool canSnap;
        private bool hasTriggerCollision;
        private Vector3 snapToObjectOffset;

        /// <summary>
        /// Update.
        /// </summary>
        private void Update()
        {
            if (Input.GetMouseButtonUp(0) && hasTriggerCollision)
            {
                canSnap = true;
            }

            if (Input.GetMouseButtonDown(0))
            {
                canSnap = false;
                hasTriggerCollision = false;
            }
        }

        /// <summary>
        /// Ons trigger enter.
        /// </summary>
        /// <param name="other">The other.</param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Snap")
            {
                if (TransformManager.Instance.SelectedObject == null)
                    return;

                if (TransformManager.Instance.SelectedObject.gameObject != other.transform.parent.parent.parent.parent.parent.gameObject)
                {
                    snapToObjectOffset = TransformManager.Instance.SelectedObject.GetWorldPosition() - transform.position;
                }
            }
        }

        /// <summary>
        /// Ons trigger stay.
        /// </summary>
        /// <param name="other">The other.</param>
        private void OnTriggerStay(Collider other)
        {
            if (other.tag == "Snap")
            {
                if (TransformManager.Instance.SelectedObject == null)
                    return;

                hasTriggerCollision = true;

                if (TransformManager.Instance.SelectedObject.gameObject != other.transform.parent.parent.parent.parent.parent.gameObject && canSnap)
                {
                    TransformManager.Instance.SelectedObject.SetWorldPosition(other.transform.position + snapToObjectOffset, 0);
                }
            }
        }

        /// <summary>
        /// Ons trigger exit.
        /// </summary>
        /// <param name="other">The other.</param>
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Snap")
            {
                if (TransformManager.Instance.SelectedObject == null)
                    return;

                if (TransformManager.Instance.SelectedObject.gameObject != other.transform.parent.parent.parent.parent.parent.gameObject)
                {
                    canSnap = false;
                    hasTriggerCollision = false;
                }
            }
        }
    }
}


