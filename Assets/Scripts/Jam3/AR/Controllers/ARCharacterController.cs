//-----------------------------------------------------------------------
// <copyright file="ARCharacterController.cs" company="Jam3 Inc">
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

namespace Jam3.AR
{
    /// <summary>
    /// AR character controller (editor AR fallback fro debug/test).
    /// </summary>
    /// <seealso cref="MonoBehaviour" />
    public class ARCharacterController : MonoBehaviour
    {
        [HideInInspector]
        public bool NeedsUpdate = true;

        [Range(0.0f, 1.0f)]
        public float MovementSmoothFactor = 0.5f;

        [Header("Character Movement (Desktop Only)")]
        [SerializeField]
        private GameObject movementObject = null;

        [Range(0.1f, 10.0f)]
        public float MoveVelocity = 3.0f;

        [Header("Mouse Control (Desktop Only)")]
        [SerializeField] private bool PressToLook = true;

        [SerializeField] private GameObject lookAtXObject = null;

        [SerializeField] private GameObject lookAtYObject = null;

        [Range(-90.0f, 0.0f)]
        public float lookAtYMinAngle = -35f;

        [Range(0.0f, 90.0f)]
        public float lookAtYMaxAngle = 75f;

        [Range(1.0f, 10.0f)]
        public float Sensitivity = 5.0f;

        [Range(0.0f, 10.0f)]
        public float Smoothing = 2.0f;

        public bool MouseInvertedY = false;
        public bool MouseInvertedX = false;

        // Runtime variables
        private bool isDown = false;
        private bool moved = false;

        private float translation;
        private float straffe;

        private Vector3 initialMovePosition = Vector3.zero;
        private Vector3 currentMovePosition = Vector3.zero;
        private Vector3 targetMovePosition = Vector3.zero;

        private Vector2 initialHoldPosition = Vector2.zero;
        private Vector2 stickAnalogValue = Vector2.zero;

        // Mouse Private
        private Vector2 mouseLook = Vector2.zero;
        private Vector2 smoothV = Vector2.zero;
        private Vector2 mouseVector = Vector2.zero;
        private Vector2 sensitivityVector = Vector2.zero;
        private Vector2 mouseDirection = Vector2.zero;

        private bool IsShooting = false;

        private Vector3 velocity = Vector3.zero;
        private bool canTap = false;
        private bool canLookAt = false;

        /// <summary>
        /// Start.
        /// </summary>
        private void Start()
        {
            mouseVector = new Vector2(0, 0);
            sensitivityVector = new Vector2(0, 0);

            mouseDirection = new Vector2(MouseInvertedX ? 1 : -1, MouseInvertedY ? 1 : -1);

            canLookAt = false;

            targetMovePosition = initialMovePosition;
            currentMovePosition = targetMovePosition;

            canTap = true;
        }

        /// <summary>
        /// Update.
        /// </summary>
        private void Update()
        {
            canTap = true;

            if (!NeedsUpdate) return;

            mouseDirection.x = MouseInvertedX ? 1 : -1;
            mouseDirection.y = MouseInvertedY ? 1 : -1;

            // Mobile/Tablet
            if (Input.touchSupported && Input.touchCount > 0)
            {
                Touch touch = Input.touches[0];

                if (touch.phase == TouchPhase.Began)
                {
                    Hold(touch.position);
                }

                if (touch.phase == TouchPhase.Moved && isDown && canTap)
                {

                }

                if (touch.phase == TouchPhase.Ended || !canTap)
                {
                    IsShooting = false;
                    Release();
                }
            }
            // Desktop
            else
            {
                float deltaX = Input.GetAxis("Mouse X");
                float deltaY = Input.GetAxis("Mouse Y");
                moved = (deltaX != 0 || deltaY != 0);

                if (Input.GetMouseButtonDown((int)MouseButtons.Left))
                {
                    if (canTap)
                        IsShooting = true;
                }

                if (Input.GetMouseButtonUp((int)MouseButtons.Left))
                {
                    IsShooting = false;
                }

                if (Input.GetMouseButtonDown((int)MouseButtons.Right))
                    Hold(Input.mousePosition);

                // if (isDown && moved) {}

                if (Input.GetMouseButtonUp((int)MouseButtons.Right))
                    Release();

                CharacterMovement();

                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    canLookAt = true;
                }

                if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    canLookAt = false;
                }

                if (canLookAt || !PressToLook)
                    LookAtMovement();
            }

            UpdateTargetPosition();
            if (IsShooting) { }
        }

        /// <summary>
        /// Updates target position.
        /// </summary>
        private void UpdateTargetPosition()
        {
            currentMovePosition = Vector3.SmoothDamp(currentMovePosition, targetMovePosition, ref velocity, MovementSmoothFactor);
            currentMovePosition.x = float.IsNaN(currentMovePosition.x) ? 0.0f : currentMovePosition.x;
            currentMovePosition.y = float.IsNaN(currentMovePosition.y) ? 0.0f : currentMovePosition.y;
            currentMovePosition.z = float.IsNaN(currentMovePosition.z) ? 0.0f : currentMovePosition.z;
        }


        /// <summary>
        /// Looks at movement.
        /// </summary>
        private void LookAtMovement()
        {
            mouseVector.x = Input.GetAxisRaw("Mouse X");
            mouseVector.y = Input.GetAxisRaw("Mouse Y");

            sensitivityVector.x = Sensitivity * Smoothing;
            sensitivityVector.y = Sensitivity * Smoothing;

            mouseVector = Vector2.Scale(mouseVector, sensitivityVector);
            smoothV.x = Mathf.Lerp(smoothV.x, mouseVector.x, 1f / Smoothing);
            smoothV.y = Mathf.Lerp(smoothV.y, mouseVector.y, 1f / Smoothing);
            mouseLook += smoothV;
            mouseLook.y = Mathf.Clamp(mouseLook.y, lookAtYMinAngle, lookAtYMaxAngle);

            if (lookAtYObject)
                lookAtYObject.transform.localRotation = Quaternion.Euler(mouseLook.y * mouseDirection.y, 0, 0);

            if (lookAtXObject)
                lookAtXObject.transform.localRotation = Quaternion.Euler(0, -mouseLook.x * mouseDirection.x, 0);
        }

        /// <summary>
        /// Characters movement.
        /// </summary>
        private void CharacterMovement()
        {
            if (movementObject)
            {
                translation = Input.GetAxis("Vertical") * MoveVelocity * Time.deltaTime;
                straffe = Input.GetAxis("Horizontal") * MoveVelocity * Time.deltaTime;
                movementObject.transform.Translate(straffe, 0, translation);
            }
        }

        /// <summary>
        /// Hold.
        /// </summary>
        /// <param name="position">The position.</param>
        private void Hold(Vector2 position)
        {
            initialHoldPosition = position;

            if (isDown && Input.touchSupported && canTap)
                IsShooting = true;
        }

        /// <summary>
        /// Release.
        /// </summary>
        private void Release()
        {
            initialHoldPosition.x = 0;
            initialHoldPosition.y = 0;

            isDown = false;
        }
    }
}
